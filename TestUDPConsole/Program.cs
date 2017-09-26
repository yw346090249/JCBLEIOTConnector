using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestUDPConsole
{
    class Program
    {
        private static UdpClient _udpClient = new UdpClient();
        private static IPEndPoint _sendEndPoint = null;
        private static IPEndPoint _localEndPoint = null;
        static void Main(string[] args)
        {
            StartReceive();
            SendAsync(System.Text.Encoding.UTF8.GetBytes("OK")).Wait();
            Console.ReadLine();
        }

        public static void StartReceive()
        {
            _localEndPoint = new IPEndPoint(IPAddress.Any, 1701);
            _sendEndPoint = new IPEndPoint(IPAddress.Parse("10.0.0.8"), 1700);
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(_localEndPoint);
            _udpClient.MulticastLoopback = true;
            _udpClient.BeginReceive(OnMessageReceived, null);
        }

        private static async void OnMessageReceived(IAsyncResult ar)
        {
            try
            {
                byte[] data = _udpClient.EndReceive(ar, ref _localEndPoint);
                _udpClient.BeginReceive(OnMessageReceived, null);
                Console.WriteLine("Received Message:");
                Console.WriteLine(Convert.ToBase64String(data));
                int x = await SendAsync(System.Text.Encoding.UTF8.GetBytes("OK"));
            }
            catch (Exception ex)
            {
                _udpClient.BeginReceive(OnMessageReceived, null);
                Console.WriteLine(ex.ToString());
            }

        }

        public static async Task<int> SendAsync(byte[] data)
        {
            return await _udpClient.SendAsync(data, data.Length, _sendEndPoint);
        }
    }
}
