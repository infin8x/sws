using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Net;
using Server.Protocol;

namespace Server
{
    class ConnectionHandler
    {
        private Server Server { get; set; }
        private Socket Socket { get; set; }

        public ConnectionHandler(Server server, Socket socket)
        {
            Server = server;
            Socket = socket;
        }

        internal void Handle()
        {
            var start = DateTime.Now;
            var stream = new NetworkStream(Socket);
            var streamReader = new StreamReader(stream);

            HttpRequest request;
            HttpResponse response = null;

            try
            {
                request = new HttpRequest(streamReader);
            }
            catch (ProtocolException e)
            {
                //if (e.Status == Constants.BadRequestCode)
                response = HttpResponseFactory.CreateBadRequest(Constants.Close);
                response.Write(stream);
                PrepareToReturn(start);
                return;
            }
            if (request.ProtocolVersion != HttpVersion.Version11)
            {
                response = HttpResponseFactory.CreateNotSupported(Constants.Close);
                response.Write(stream);
                PrepareToReturn(start);
                return;
            }

            if (request.Method != "GET")
            {
                // TODO: proper error handling
                PrepareToReturn(start);
                return;
            }

            // request is GET from here on
            FileStream requestedFile;
            var path = Server.RootDirectory + request.Uri;
            if (Directory.Exists(path))
                path += "\\" + Constants.DefaultFile;
            if (!File.Exists(path))
                response = HttpResponseFactory.CreateNotFound(Constants.Close);
            else // file or directory (i.e. default file) exists; open it
                response = HttpResponseFactory.CreateOk(path, Constants.Close);
            response.Write(stream);
            // TODO: Support 304

            PrepareToReturn(start);
        }



        private void PrepareToReturn(DateTime start)
        {
            Socket.Close();
            Server.Connections++;
            Server.ServiceTime += (DateTime.Now - start).TotalMilliseconds;
        }
    }
}
