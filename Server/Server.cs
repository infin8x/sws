using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Server
    {
        public string RootDirectory { get; private set; }


        public long Connections { get; set; }
        public double ServiceTime { get; set; }

        protected bool Listening { get; set; }
        public TcpListener Listener { get; private set; }
        public Thread ListenThread { get; set; }


        static void Main(string[] args)
        {
            var server = new Server("C:\\_sws", 8080);
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
                var client = Listener.AcceptSocket();
                // pass control to ConnectionHandler.Handle
                var handler = new ConnectionHandler(this, client);
                new Thread(handler.Handle).Start();
            }
            Listener.Stop();
        }


        public double GetServiceRate()
        {
            if (ServiceTime.CompareTo(0.0) == 0) return long.MinValue;
            var rate = Connections / (double)ServiceTime;
            return rate * 1000;
        }
    }
}

