using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public string RootDirectory { get; private set; }

        public long Connections { get; private set; }

        public long ServiceTime { get; private set; }

        public Thread ListenThread { get; set; }
        public TcpListener TCPListener { get; private set; }

        public Server(String rootDirectory, int port)
        {
            RootDirectory = rootDirectory;

            Connections = 0;
            ServiceTime = 0;

            TCPListener = new TcpListener(IPAddress.Any, port);
            ListenThread = new Thread(ListenForClients);
            ListenThread.Start();
        }



        private void ListenForClients()
        {
            TCPListener.Start();
            while (true)
            {
                // block until client connects
                var client = TCPListener.AcceptTcpClient();

                new Thread(HandleClientRequest).Start(client);
            }
        }

        static void Main(string[] args)
        {
            var server = new Server("C:\\", 3000);
        }

        public double GetServiceRate()
        {
            if (ServiceTime == 0) return long.MinValue;
            var rate = Connections / (double)ServiceTime;
            return rate * 1000;
        }

        private void HandleClientRequest(object inc)
        {
            var client = (TcpClient) inc;
            

        }


     
    }
}
