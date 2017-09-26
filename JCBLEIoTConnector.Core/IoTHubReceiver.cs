/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using log4net;

namespace JCBLEIoTConnector.Core
{
    public class IoTHubReceiver : IDisposable
    {
        #region --- Fields ---
        private EventHubClient _eventHubClient = null;
        private List<EventHubReceiver> _eventHubReceiverList = null;
        private int _eventHubPartitionsCount = 0;
        private UDPConnector _UDPConnector = null;
        private StorageConnector _storageConnector = null;
        private ILog _infoLogger = null;
        private ILog _errorLogger = null;
        #endregion

        #region --- Constructros ---
        public IoTHubReceiver()
        {
            try
            {
                _infoLogger = LogManager.GetLogger(this.GetType());
                _errorLogger = LogManager.GetLogger(this.GetType());
                _storageConnector = new StorageConnector(SettingsManager.StorageConnectionString);
                _infoLogger.Info("Initialize IoTHubReceiver.");
                _UDPConnector = new UDPConnector(SettingsManager.NSServer,
                    SettingsManager.UDPPort, SettingsManager.LocalUDPPort);
                _infoLogger.InfoFormat("Initialize IoTHubReceiver.");
                _UDPConnector.UDPDataReceived += UDPConnector_UDPDataReceived;
                _infoLogger.InfoFormat("Attached event handler to UDPConnector.");
                _UDPConnector.Open();
                _infoLogger.InfoFormat("UDPConnector Initialized.");
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("IoTHubReceiver constructor got error:{0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }
        // This function respond for retrieve data from UDP port.
        private void UDPConnector_UDPDataReceived(object sender, byte[] data)
        {
            try
            {
                _infoLogger.InfoFormat("UDP port received data:{0}", Convert.ToBase64String(data));
                // <TODO>: a) Convert decrypted data to data model object.

                // <TODO>: b) Save descryped data to Azure Storage Account table.
                // _storageConnector.SaveDataToTableAsync(null).Wait();
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("UDPConnector_UDPDataReceived function got error when handle data from UDP port:{0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }
        #endregion

        #region --- Methods ---
        public async void ReceiveEventDataAsync(DateTime startTime)
        {
            _infoLogger.Info("Starting retrieve data from IoT Hub......");
            _eventHubReceiverList = new List<EventHubReceiver>();
            _eventHubClient = EventHubClient.CreateFromConnectionString(SettingsManager.EventHubConnectionString);
            _infoLogger.Info("Created EventHubClient from IoTHub connection string.");
            _eventHubPartitionsCount = _eventHubClient.GetRuntimeInformation().PartitionCount;
            string[] partitionIds = _eventHubClient.GetRuntimeInformation().PartitionIds;
            _infoLogger.Info("Get EventHubClient partition informations.");
            try
            {
                // Crate event hub receviers by partition id.
                foreach (string pId in partitionIds)
                {
                    if (-1 < _eventHubClient.GetPartitionRuntimeInformation(pId).LastEnqueuedSequenceNumber)
                    {
                        // Receive data from startTime
                        _eventHubReceiverList.Add(_eventHubClient.GetDefaultConsumerGroup().CreateReceiver(pId, startTime));
                        _infoLogger.InfoFormat("{0}: Partition {1} has queued data.Getting ready to retrieve data.", startTime, pId);
                    }
                }
                // Retrieve event data from partitions.
                _infoLogger.InfoFormat("Begin retrieve histrical data from IoT Hub.");
                List<EventData> dataList = new List<EventData>();
                // Retrieve histrical data.
                foreach (EventHubReceiver recv in _eventHubReceiverList)
                {
                    dataList.AddRange(
                        await recv.ReceiveAsync(int.MaxValue - 1, TimeSpan.FromSeconds(20)));
                    _infoLogger.InfoFormat("Collected historical data from EventHubReceiver {0}.", recv.Name);
                }
                long historicalCount = 0;
                foreach (EventData data in dataList)
                {
                    // Send event data to NS service port UDP port for decrypt.
                    var enqueuedTime = data.EnqueuedTimeUtc.ToLocalTime();
                    _infoLogger.InfoFormat("Time: {0}\t Data Received: {1}", 
                        enqueuedTime.ToString(), Convert.ToBase64String(data.GetBytes()));
                    await _UDPConnector.SendAsync(data.GetBytes());
                    historicalCount++;
                }
                _infoLogger.InfoFormat("Totally sent {0} messages to UDP port.", historicalCount);
                _infoLogger.InfoFormat("End retrieve histrical data from IoT Hub.");
                List<EventData> currentDataList = null;
                long currenctCount = 0;
                _infoLogger.InfoFormat("Begin retrieve current data from IoT Hub.");
                while (true)
                {
                    currentDataList = new List<EventData>();
                    foreach (EventHubReceiver recv in _eventHubReceiverList)
                    {
                        currentDataList.AddRange(await recv.ReceiveAsync(int.MaxValue, TimeSpan.FromSeconds(1)));
                        _infoLogger.InfoFormat("Collected current data from EventHubReceiver {0}.", recv.Name);
                    }

                    foreach (EventData data in currentDataList)
                    {
                        // Send event data to NS service port UDP port for decrypt.
                        var enqueuedTime = data.EnqueuedTimeUtc.ToLocalTime();
                        await _UDPConnector.SendAsync(data.GetBytes());
                        currenctCount++;
                    }
                    _infoLogger.InfoFormat("This loop has been sent {0} messages to UDP port.", currenctCount);
                    // clear all.
                    currenctCount = 0;
                    currentDataList = null;
                }
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("ReceiveEventDataAsync got an error when retrieve data from IoT Hub:{0}",
                    ex.ToString().Replace("\r\n", " "));
                try
                {
                    _infoLogger.InfoFormat("Now reconnecting IoT Hub.");
                    // If IoTHub disconnected then reconnect.
                    await CloseAsync();
                    ReceiveEventDataAsync(DateTime.Now);
                }
                catch
                {
                }
            }
            _infoLogger.InfoFormat("End retrieve current data from IoT Hub.");
        }

        public async Task CloseAsync()
        {
            try
            {
                // Close EventHubReceiver
                foreach (EventHubReceiver recv in _eventHubReceiverList)
                {
                    await recv.CloseAsync();
                }

                _eventHubReceiverList.Clear();
                _infoLogger.InfoFormat("EventHubReceiver has been closed.");
                // Close EventHubClient
                await _eventHubClient.CloseAsync();
                _infoLogger.InfoFormat("EventHubClient has been closed.");
                // Close UDP socket
                // _UDPConnector.Close();
                _infoLogger.InfoFormat("UDPConnector has been closed.");
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("IoTHubReceiver closing got an error: {0}", 
                    ex.ToString().Replace("\r\n", " "));
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected async virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    await this.CloseAsync();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~IoTHubReceiver() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
