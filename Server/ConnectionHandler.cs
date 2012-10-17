using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net;

namespace Server
{
    class ConnectionHandler
    {
        public Server Server { get; private set; }
        public TcpClient Client { get; set; }

        public ConnectionHandler(Server server, TcpClient client)
        {
            Server = server;
            Client = client;
        }
        internal void Handle()
        {
            using(var timer = new Timer())
            {
                timer.Start();
                using (var stream = Client.GetStream())
                {
                    if (stream.CanRead)
                    {
                        var listener= new HttpListener();
                        listener.
                    }
                }
            }

        }

        
    }
}
