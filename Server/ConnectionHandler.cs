using System;
using System.IO;
using System.Timers;
using System.Net;

namespace Server
{
    class ConnectionHandler
    {
        private Server Server { get; set; }

        public ConnectionHandler(Server server)
        {
            Server = server;
            while (true)
            {
                Listen();
            }
        }

        private void Listen()
        {
            var start = DateTime.Now;
            // wait for request
            var context = Server.Listener.GetContext();
            var request = context.Request;
            var response = context.Response;

            if (request.ProtocolVersion != HttpVersion.Version11)
            {
                // TODO: proper error handling    
                return;
            }
            
            if (request.HttpMethod != "GET")
            {
                // TODO: proper error handling
                return;
            }

            // request is GET from here on
            FileStream requestedFile;
            var path = Server.RootDirectory + request.RawUrl;

            if (Directory.Exists(path))
            {
                path += "\\" + "index.html";
                // TODO: DEFAULT_FILE constant
            }
            if (!File.Exists(path))
            {
                HttpListenerResponseFactory.CreateNotFound(response);
            }
            // file or directory (i.e. default file) exists; open it
            requestedFile = File.Open(path, FileMode.Open);
            HttpListenerResponseFactory.CreateOk(response);
            
            // write response and close socket
            response.Close();

            IncrementStatistics(start);
        }

        private void IncrementStatistics(DateTime start)
        {
            Server.Connections++;
            Server.ServiceTime += (DateTime.Now - start).TotalMilliseconds;
        }
    }
}
