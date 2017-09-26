using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCBLEIoTConnector.Core;
using Newtonsoft.Json;
using JCBLEIoTConnector.Core.Models;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Write(DateTime.Now.ToString("yyyy/MM/dd"));
            //Console.ReadLine();
            //return;
            IoTHubConnector connector = new IoTHubConnector();
            connector.StartService();

            Console.ReadLine();
            connector.StopService();
        }
    }
}
