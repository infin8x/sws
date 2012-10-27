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
        public NetworkStream Stream { get; set; }

        public ConnectionHandler(Server server, Socket socket)
        {
            Server = server;
            Socket = socket;
        }

        internal void Handle()
        {
            var start = DateTime.Now;
            Stream = new NetworkStream(Socket);
            var streamReader = new StreamReader(Stream);

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
                response.Write(Stream);
                PrepareToReturn(start);
                return;
            }

            if (request.ProtocolVersion != HttpVersion.Version11)
            {
                response = HttpResponseFactory.CreateNotSupported(Constants.Close);
                response.Write(Stream);
                PrepareToReturn(start);
                return;
            }

            if (request.Method != "GET")
            {
                response = HttpResponseFactory.CreateNotImplemented(Constants.Close);
                response.Write(Stream);
                PrepareToReturn(start);
                return;
            }

            // request is GET from here on
            var path = Server.RootDirectory + request.Uri;
            if (Directory.Exists(path))
                path += "\\" + Constants.DefaultFile;
            if (!File.Exists(path))
                response = HttpResponseFactory.CreateNotFound(Constants.Close);
            else {
                if (request.Headers.ContainsKey("If-Modified-Since"))
                {
                    DateTime ifModifiedSince = DateTime.Parse(request.Headers["If-Modified-Since"]);
                    DateTime lastModified = File.GetLastWriteTime(path); // If-Modified-Since seems to round to the nearest minute
                    lastModified = lastModified.AddTicks(-(lastModified.Ticks % TimeSpan.TicksPerMillisecond));
                    lastModified = lastModified.AddTicks(-(lastModified.Ticks % TimeSpan.TicksPerSecond)); 
                    if (lastModified.CompareTo(ifModifiedSince) <= 0)
                    {
                        response = HttpResponseFactory.CreateNotModified(Constants.Close);
                    }
                    else
                    {
                        response = HttpResponseFactory.CreateOk(path, Constants.Close);
                    }
                }
            }
            response.Write(Stream);

            PrepareToReturn(start);
        }



        private void PrepareToReturn(DateTime start)
        {
            Stream.Close();
            Socket.Close(); // this should close the NetworkStream and StreamReader as well
            Server.IncrementStatistics(start);
        }


    }
}
