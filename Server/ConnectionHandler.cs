using System;
using System.Collections.Generic;
using System.IO;
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
        public HttpListener Listener { get; set; }

        public ConnectionHandler(HttpListener listener)
        {
            Listener = listener;
            while (true)
            {
                Listen();
            }
        }

        private void Listen()
        {
            // wait for request
            var context = Listener.GetContext();

            var request = context.Request;
            var response = context.Response;

            if (request.ProtocolVersion != HttpVersion.Version11) return;
            // TODO: proper error handling

            if (request.HttpMethod != "GET") return;
            // TODO: proper error handling
            // request is GET from here on
            response.StatusCode = (int)HttpStatusCode.NotFound;
            response.ContentType = "text/html";
            response.ContentLength64 = 0;
            response.Close();



        }

    }
}
