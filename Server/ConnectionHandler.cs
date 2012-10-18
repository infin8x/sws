using System;
using System.IO;
using System.Threading;
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
                Console.WriteLine("Listen called");
                Thread.Sleep(1000);
            }
        }

        private void Listen()
        {
            var start = DateTime.Now;
            HttpListenerContext context;
            try
            {
                // wait for request
                context = Server.Listener.GetContext();
            }
            catch (HttpListenerException e)
            {
                // TODO: log for further analysis, or handle exception
                // TODO: currently, 400s are handled automatically by c# - we should override if possible
                IncrementStatistics(start);
                throw e;
            }
            var request = context.Request;
            var response = context.Response;

            if (request.ProtocolVersion != HttpVersion.Version11)
            {
                // TODO: proper error handling - currently ceases to function after return
                response.Close();
                IncrementStatistics(start);
                return;
            }

            if (request.HttpMethod != "GET")
            {
                // TODO: proper error handling
                IncrementStatistics(start);
                return;
            }

            // request is GET from here on
            FileStream requestedFile;
            var path = Server.RootDirectory + request.RawUrl;
            if (Directory.Exists(path))
                path += "\\" + Protocol.DefaultFile;
            if (!File.Exists(path))
                HttpListenerResponseFactory.CreateNotFound(response, Protocol.Close);
            // file or directory (i.e. default file) exists; open it
            using (requestedFile = File.Open(path, FileMode.Open))
            {
                HttpListenerResponseFactory.CreateOk(response, new FileInfo(path), 
                    requestedFile.Length, Protocol.Close);
                WriteFileToOutputStream(requestedFile, response.OutputStream);
                response.Close();
            }

            // TODO: Support 505 and 304

            IncrementStatistics(start);
        }

        private void WriteFileToOutputStream(FileStream requestedFile, Stream outputStream)
        {
            var buffer = new byte[Protocol.ChunkLength];
            var bytesToRead = requestedFile.Length;
            var bytesRead = 0;
            while (bytesToRead > 0)
            {
                var n = requestedFile.Read(buffer, bytesRead, Protocol.ChunkLength);
                if (n == 0) break; // EOF
                bytesRead += n;
                bytesToRead -= n;
                outputStream.Write(buffer, 0, bytesRead);
            }
            outputStream.Close();
        }

        private void IncrementStatistics(DateTime start)
        {
            Server.Connections++;
            Server.ServiceTime += (DateTime.Now - start).TotalMilliseconds;
        }
    }
}
