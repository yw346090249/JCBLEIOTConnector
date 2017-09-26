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
using System.Configuration;
using log4net;

namespace JCBLEIoTConnector.Core
{
    public class SettingsManager
    {
        #region --- Properties ---
        public static int LocalUDPPort
        {
            get { return int.Parse(ConfigurationManager.AppSettings["LocalUDPPort"]); }
        }
        public static string NSServer
        {
            get { return ConfigurationManager.AppSettings["NSServer"]; }
        }
        public static int UDPPort
        {
            get { return int.Parse(ConfigurationManager.AppSettings["NSUDPPort"]); }
        }
        public static int TCPPort
        {
            get { return int.Parse(ConfigurationManager.AppSettings["TCPPort"]); }
        }
        public static string IotHubName
        {
            get { return ConfigurationManager.AppSettings["IoTHubName"]; }
        }
        public static string IoTHubConnectionString
        {
            get { return ConfigurationManager.AppSettings["IoTHubConnectionString"]; }
        }

        public static string EventHubConnectionString
        {
            get { return ConfigurationManager.AppSettings["EventHubConnectionString"]; }
        }

        public static string EventHubDataSaveConnectionString
        {
            get { return ConfigurationManager.AppSettings["EventHubSaveDataConnectionString"]; }
        }

        public static string EventHubName
        {
            get { return ConfigurationManager.AppSettings["EventHubName"]; }
        }

        public static string StorageConnectionString
        {
            get { return ConfigurationManager.AppSettings["StorageConnectionString"]; }
        }
        public static ILog ErrorLogger
        {
            get { return LogManager.GetLogger("logerror"); }
        }
        public static ILog InfoLogger
        {
            get { return LogManager.GetLogger("loginfo"); }
        }
        #endregion
    }
}
