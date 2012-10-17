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


        private static void Main(string[] args)
        {

            if (!HttpListener.IsSupported) return;

            var listener = new HttpListener();
            listener.Prefixes.Add("http://+:8080/");

            listener.Start();

            new ConnectionHandler(listener);
        }
    }
}
