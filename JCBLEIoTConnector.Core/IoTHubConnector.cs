/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>

using log4net;
using System;
using Microsoft.Azure.Devices;


namespace JCBLEIoTConnector.Core
{
    public class IoTHubConnector
    {
        #region --- Private memebers ---
        private ILog _infoLogger = null;
        private ILog _errorLogger = null;
        private IoTHubReceiver _IoTHubReceiver = null;
        private TCPReceiver _tcpReceiver = null;
        private ServiceClient _IoTHubSender = null;
        private StorageConnector _storageConnector = null;
        #endregion

        #region --- Constructors ---
        static IoTHubConnector()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public IoTHubConnector()
        {
            _infoLogger = LogManager.GetLogger(this.GetType());
            _errorLogger = LogManager.GetLogger(this.GetType());
        }
        #endregion

        #region --- Methods ---
        public void StartService()
        {
            try
            {
                _infoLogger.Info("IoTHubConnector starting......");
                _infoLogger.InfoFormat("IoT Hub connection string:{0}", SettingsManager.IoTHubConnectionString);
                _IoTHubReceiver = new IoTHubReceiver();
                _infoLogger.Info("IoTHubReceiver created.");
                _storageConnector = new StorageConnector(SettingsManager.StorageConnectionString);
                _storageConnector.Open();
                _tcpReceiver = new TCPReceiver(ref _storageConnector);
                _tcpReceiver.Open();
                _infoLogger.Info("TCPReceiver created.");
                _IoTHubReceiver.ReceiveEventDataAsync(DateTime.Now);
                _infoLogger.Info("Begining receive message from IoT Hub.");
                _infoLogger.Info("IoTHubConnector start service completed.");
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("IoTHubConnector StartService got error:{0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }

        public void StopService()
        {
            try
            {
                _infoLogger.Info("IoTHubConnector closing......");
                _IoTHubReceiver.CloseAsync().Wait();
                _infoLogger.Info("IoTHubReceiver closed.");
                _infoLogger.Info("TCPReceiver closing......");
                _tcpReceiver.Close();
                _infoLogger.Info("TCPReceiver closed.");
                _infoLogger.Info("IoTHubConnector uninitialize completed.");
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("IoTHubConnector StartService got error:{0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }
        #endregion
    }
}
