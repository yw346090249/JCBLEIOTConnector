using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using log4net;

namespace JCBLEIoTConnector.Core
{
    class EventHubSender
    {
        #region ---field---
        private EventHubClient _eventhubClient = null;
        private ILog _infoLogger = null;
        private ILog _errorLogger = null;
        #endregion 

        #region ---Constructor---
        public EventHubSender()
        {
            try
            {
                _infoLogger = LogManager.GetLogger(this.GetType());
                _errorLogger = LogManager.GetLogger(this.GetType());
                _eventhubClient = EventHubClient.CreateFromConnectionString(
                    SettingsManager.EventHubDataSaveConnectionString, SettingsManager.EventHubName);
                _infoLogger.Info("Initialize EventHubSender.");
                _infoLogger.InfoFormat("Initialize EventHubSender.");
            }
            catch (Exception ex)
            {
                _errorLogger.ErrorFormat("EventHubSender constructor got error:{0}",
                    ex.ToString().Replace("\r\n", " "));
            }
        }
        #endregion

        #region ---- Methods -----
        public async Task SendAsync(byte[] message)
        {
            await _eventhubClient.SendAsync(new EventData(message));
        }
        #endregion 
    }
}