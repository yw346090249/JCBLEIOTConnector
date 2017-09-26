using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using log4net;

namespace JCBLEIoTConnector.Core
{
    class IoTHubSender
    {
        #region -----field------
        private ServiceClient _serviceClient = null;
        private ILog _infoLogger = null;
        private ILog _errorLogger = null;
        #endregion

        #region --- Constructors ---
        public IoTHubSender()
        {
            try
            {
                _infoLogger = LogManager.GetLogger(this.GetType());
                _errorLogger = LogManager.GetLogger(this.GetType());
                _serviceClient = ServiceClient.CreateFromConnectionString(SettingsManager.IoTHubConnectionString);
                _infoLogger.Info("Initialize IoTHubSender.");
                _infoLogger.InfoFormat("Initialize IoTHubSender.");
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("IoTHubSender constructor got error:{0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }
        #endregion

        #region ---- Methods -----
        public async Task SendAsync(byte[] data, string EUI)
        {
            try
            {
                var message = new Message(data);
                //Console.WriteLine("Send Data to Gateway {0} : {1}", EUI, message);
                await _serviceClient.SendAsync(EUI, message);
                _infoLogger.InfoFormat("Send Data to Gateway {0} : {1}", EUI, message);
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("IoTHub sending data to Gateway got an error: {0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }
        #endregion
    }
}
