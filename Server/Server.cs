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

        protected bool Listening { get; set; }
        public TcpListener Listener { get; private set; }
        public Thread ListenThread { get; set; }

        static void Main(string[] args)
        {
            var server = new Server("C:\\", 8080);
            //Thread.Sleep(5000);
            //server.Listening = false;
        }

        public Server(String rootDirectory, int port)
        {
            RootDirectory = rootDirectory;

            Connections = 0;
            ServiceTime = 0;

            Listening = true;
            Listener = new TcpListener(IPAddress.Any, port);
            ListenThread = new Thread(Listen);
            ListenThread.Start();
        }

        private void Listen()
        {
            Listener.Start();
            while (Listening)
            {
                // block until client connects
                var client = Listener.AcceptTcpClient();
                // pass control to ConnectionHandler.Handle
                var handler = new ConnectionHandler(this, client);

                new Thread(handler.Handle).Start();
            }
            Listener.Stop();
        }

        public double GetServiceRate()
        {
            if (ServiceTime == 0) return long.MinValue;
            var rate = Connections / (double)ServiceTime;
            return rate * 1000;
        }
    }
}
