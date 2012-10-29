using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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

            if (CheckDdos(Socket))
            {
                PrepareToReturn(start);
                return;
            }
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
            else
            {
                if (request.Headers.ContainsKey("If-Modified-Since"))
                {
                    var ifModifiedSince = DateTime.Parse(request.Headers["If-Modified-Since"]);
                    var lastModified = File.GetLastWriteTime(path); // If-Modified-Since seems to round to the nearest minute
                    lastModified = lastModified.AddTicks(-(lastModified.Ticks % TimeSpan.TicksPerMillisecond));
                    lastModified = lastModified.AddTicks(-(lastModified.Ticks % TimeSpan.TicksPerSecond));
                    response = lastModified.CompareTo(ifModifiedSince) <= 0 ?
                        HttpResponseFactory.CreateNotModified(Constants.Close) :
                        HttpResponseFactory.CreateOk(path, Constants.Close);
                }
                else
                {
                    response = HttpResponseFactory.CreateOk(path, Constants.Close);
                }
            }
            if (request.Headers.ContainsKey(Constants.Connection))
                response.Headers[Constants.Connection] = request.Headers[Constants.Connection];
            try
            {
                response.Write(Stream);
            }
            catch (Exception e)
            { return; }
            if (response.Headers[Constants.Connection] != Constants.Open)
                PrepareToReturn(start);
            else
                Handle();
        }

        private bool CheckDdos(Socket socket)
        {
            var endpoint = Socket.RemoteEndPoint as IPEndPoint;
            if (endpoint == null) return true;
            lock (Server.ConnectionCount)
            {
                if (Server.ConnectionCount.ContainsKey(endpoint.Address))
                {
                    Server.ConnectionCount[endpoint.Address]++;
                }
                else
                {
                    Server.ConnectionCount.Add(endpoint.Address, 1);
                }
                return Server.ConnectionCount[endpoint.Address] > 100;
            }
        }

        private void SubtractConnectionCount(Socket socket)
        {
            if (Socket.RemoteEndPoint as IPEndPoint == null) return;
            lock (Server.ConnectionCount)
            {
                var endpoint = Socket.RemoteEndPoint as IPEndPoint;
                if (Server.ConnectionCount.ContainsKey(endpoint.Address))
                {
                    Server.ConnectionCount[endpoint.Address]--;
                }
            }
        }


        private void PrepareToReturn(DateTime start)
        {
            SubtractConnectionCount(Socket);

            Socket.Close(); // this should close the NetworkStream and StreamReader as well
            Server.IncrementStatistics(start);
        }
    }
}
