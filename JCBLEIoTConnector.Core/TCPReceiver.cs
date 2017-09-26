
using JCBLEIoTConnector.Core.Models;
/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>
using log4net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace JCBLEIoTConnector.Core
{
    public class TCPReceiver
    {
        #region --- Private members ---
        private TcpListener _tcpListener = null;
        private ILog _infoLogger = null;
        private ILog _errorLogger = null;
        private StorageConnector _storageConnector = null;
        private TcpClient _tcpClient = null;
        private bool _isRunnning = false;

        private EventHubSender _eventhubSender = null;
        #endregion

        #region --- Constructors ---
        public TCPReceiver(ref StorageConnector storageConnector)
        {
            _storageConnector = storageConnector;
            _tcpListener = new TcpListener(
                IPAddress.Any , SettingsManager.TCPPort);
            _infoLogger = LogManager.GetLogger(this.GetType());
            _errorLogger = LogManager.GetLogger(this.GetType());

            _eventhubSender = new EventHubSender();
        }
        #endregion

        public void Open()
        {
            try
            {
                _isRunnning = true;
                _tcpListener.Start();
                _infoLogger.InfoFormat("Starting listen TCP port {0}.", SettingsManager.TCPPort.ToString());
                _tcpListener.BeginAcceptTcpClient(OnTCPDataReceived, null);
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("Listening TCP port encount error : {0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }

        public void Close()
        {
            try
            {
                _isRunnning = false;
                Thread.Sleep(100);
                if(null != _tcpClient)
                    _tcpClient.Close();
                _infoLogger.Info("Stop listening TCP port.");
                _tcpListener.Stop();
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("Stop listening TCP port encount error : {0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }

        private void OnTCPDataReceived(IAsyncResult ar)
        {
            #region --- Short linke ---
            //try
            //{
            //    _infoLogger.Info("TCP port received data.");
            //    TcpClient client = _tcpListener.EndAcceptTcpClient(ar);
            //    byte[] buffer = new byte[client.ReceiveBufferSize];
            //    NetworkStream ns = client.GetStream();
            //    ns.ReadAsync(buffer, 0, buffer.Length).Wait();
            //    ns.Close();
            //    client.Close();
            //    _infoLogger.InfoFormat("TCP port received data : {0}", Convert.ToBase64String(buffer));
            //    // Console.WriteLine("TCP port received data : {0}", Convert.ToBase64String(buffer));
            //    // <TODO>: I don't even known what's TCP data meaning. So I cannot handle this by now.
            //    // It's need to handle received data here. You need Deserialize buffer data.

            //    // _tcpListener.BeginAcceptTcpClient(OnTCPDataReceived, null);
            //}
            //catch (Exception ex)
            //{
            //    _errorLogger.ErrorFormat("Handling TCP received data encounter error: {0}", 
            //        ex.ToString().Replace("\r\n", " "));
            //}
            #endregion

            try
            {
                _infoLogger.Info("TCP port establish connection with NS server.");
                _tcpClient = _tcpListener.EndAcceptTcpClient(ar);
                byte[] buffer = new byte[2048];
                NetworkStream ns = _tcpClient.GetStream();
                string json = "";
                while (_isRunnning)
                {
                    // reset buffer.
                    for (int i = 0; i < 2048; i++)
                        buffer[i] = 0;
                    ns.ReadAsync(buffer, 0, 2048).Wait();
                    int flag = 0;
                    for (int i = 0; i < 2048; i++)
                    {
                        if(buffer[i]!=0)
                        {
                            flag = 1;
                            break;
                        }
                    }
                    
                    if (flag > 0)
                    {
                        _infoLogger.InfoFormat("TCP port received data : {0}", Convert.ToBase64String(buffer));

                        List<byte> lbuffer = new List<byte>();

                        for (int i = 0; i < 2048; i++)
                        {
                            if(buffer[i] != 0)
                            {
                                lbuffer.Add(buffer[i]);
                            }
                        }

                        byte[] buff = new byte[lbuffer.Count];
                        for (int i = 0; i < lbuffer.Count; i++)
                        {
                            buff[i] = lbuffer[i];
                        }

                        //<TODO>: buffer => MeterDataEntry
                        json = Encoding.Default.GetString(buff);

                        MeterDataEntry meterData = new MeterDataEntry();
                        meterData = JsonConvert.DeserializeObject<MeterDataEntry>(json);
                        FinalDataEntry finalData = new FinalDataEntry();
                        finalData.originData = meterData;
                        if (meterData != null && meterData.app != null)
                        {
                            string dtime = meterData.app.gwrx[0].time;
                            string pl = meterData.app.userdata.payload;
                            int tail = 4 - (pl.Length) % 4;
                            if (tail < 4)
                            {
                                for (int k = 0; k < tail; k++)
                                    pl += "=";
                            }
                            byte[] payload = Convert.FromBase64String(pl);
                            Payload payloadData = new Payload(
                                (int)payload[0],
                                (int)payload[1],
                                BitConverter.ToInt16(payload.Skip(2).Take(4).ToArray(), 0) / 1000.0f,
                                BitConverter.ToInt16(payload.Skip(6).Take(4).ToArray(), 0) / 1000.0f,
                                BitConverter.ToInt16(payload.Skip(10).Take(4).ToArray(), 0) / 1000.0f,
                                BitConverter.ToInt16(payload.Skip(14).Take(2).ToArray(), 0),
                                Encoding.ASCII.GetString(payload.Skip(16).Take(4).ToArray()),
                                dtime);
                            finalData.userData = payloadData;
                            string finalData_json = JsonConvert.SerializeObject(finalData);
                            _storageConnector.SaveDataToBlobAsync(finalData_json).Wait();
                            _eventhubSender.SendAsync(Encoding.UTF8.GetBytes(finalData_json)).Wait();
                            _infoLogger.InfoFormat("Buffer data : {0} has been written to Blob Storage.", Convert.ToBase64String(buffer));
                        }
                    }
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("Handling TCP received data encounter error: {0}",
                    ex.ToString().Replace("\r\n", " "));

                try
                {
                    Close();
                    Open();
                }
                catch
                {
                }
            }
        }
    }
}
