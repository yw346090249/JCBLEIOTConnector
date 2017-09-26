/// <summary>
/// Author: Micheal Li, Zepeng She, David Yan
/// Project: JCBLE Ascend+ Project
/// Start: Nov 13, 2016
/// End:
/// Function: Connection and API adaptor between IoT hub and NS provided by JCBLE
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using log4net;

namespace JCBLEIoTConnector.Core
{
    public class UDPConnector
    {
        #region --- Fields ---
        private UdpClient _client = null;
        private IPEndPoint _sendEndPoint = null;
        private IPEndPoint _localEndPoint = null;
        private string _ip = string.Empty;
        private int _port = 0;
        private ILog _infoLogger = null;
        private ILog _errorLogger = null;
        private IoTHubSender _iotHubSender = null;
        #endregion

        #region --- Events & Delegates ---
        public delegate void UDPDataReceivedEventHandler(object sender, byte[] data);
        public event UDPDataReceivedEventHandler UDPDataReceived;
        #endregion

        #region ---  Constructors ---
        public UDPConnector(string ip, int port, int localPort)
        {
            _client = new UdpClient();
            _ip = ip;
            _port = port;
            _localEndPoint = new IPEndPoint(IPAddress.Any, localPort);
            _sendEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            _infoLogger = LogManager.GetLogger(this.GetType());
            _errorLogger = LogManager.GetLogger(this.GetType());
            _iotHubSender = new IoTHubSender();
        }
        #endregion

        #region --- Methods ---
        public void Open()
        {
            try
            {
                _infoLogger.Info("UDPConnector executing Open function.");
                _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _client.Client.Bind(_localEndPoint);
                _client.MulticastLoopback = true;
                _infoLogger.Info("Begining recetive UDP data......");
                _client.BeginReceive(OnMessageReceived, null);
                _infoLogger.Info("UDPConnector executing Open completed.");
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("UDPConnector open port got an error:{0}", 
                    ex.ToString().Replace("\r\n", " "));
            }

        }

        public void Close()
        {
            _client.Close();
            _infoLogger.Info("UDPClient object closed.");
        }

        public async Task<int> SendAsync(byte[] data)
        {
            try
            {
                //Console.WriteLine("UDP Send Data Succeed", _sendEndPoint.ToString(), Convert.ToBase64String(data), DateTime.Now);
                _infoLogger.InfoFormat("UDPConnector sending data to {0}: {1}",
                    _sendEndPoint.ToString(), Convert.ToBase64String(data));
                return await _client.SendAsync(data, data.Length, _sendEndPoint);
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("UDPConnector sending data got an error: {0}", 
                    ex.ToString().Replace("\r\n", " "));
            }
            return -1;
        }
        #endregion

        private void OnMessageReceived(IAsyncResult ar)
        {
            try
            {
                byte[] data = _client.EndReceive(ar, ref _localEndPoint);
                _infoLogger.InfoFormat("UDP port got data:{0}", Convert.ToBase64String(data));
                //Console.WriteLine("UDP port got data:{0}", Convert.ToBase64String(data));
                _client.BeginReceive(OnMessageReceived, null);
                _infoLogger.InfoFormat("UDP port receive data again.");
                _infoLogger.InfoFormat("Invoking event handler.");

                // Gateway EUI
                byte[] EUI = new byte[8];
                for(int i = 0; i < 8; i++)
                {
                    EUI[i] = data[i + 4];
                }
                string hexEUI = BitConverter.ToString(EUI).Replace("-", string.Empty);
                _infoLogger.InfoFormat("Gateway EUI: {0}", hexEUI);
                // Send data to Device hexEUI
                _iotHubSender.SendAsync(data, hexEUI).Wait();

                //if (null != UDPDataReceived)
                //    UDPDataReceived(this, data);
                _infoLogger.InfoFormat("UDP data handled completed.");
            }
            catch(Exception ex)
            {
                _errorLogger.ErrorFormat("UDPConnector receiving data got an error : {0}", 
                    ex.ToString().Replace("\r\n", " "));
            }
        }
    }
}
