using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class Server
    {
        public HttpListener Listener { get; private set; }
        public int Connections { get; set; }
        public double ServiceTime { get; set; }
        public String RootDirectory { get; set; }

        private static void Main(string[] args)
        {
            
            new Server("C:\\_sws");
        }

        public Server(String rootDirectory)
        {
            if (!HttpListener.IsSupported) return;
            RootDirectory = rootDirectory;
            Connections = 0;

            Listener = new HttpListener();
            Listener.Prefixes.Add("http://+:8080/");
            
            Listener.Start();
            new ConnectionHandler(this);
        }
    }
}
