using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace Server
{
    public class Server
    {
        public string RootDirectory { get; private set; }

        protected bool Listening { get; set; }
        public TcpListener Listener { get; private set; }

        private Int64 _connections;
        private Int64 _serviceTime;
        
        public Server(String rootDirectory, int port)
        {
            RootDirectory = rootDirectory;
            _connections = 0;
            _serviceTime = 0;

            Start(port);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public String GetServiceRate()
        {
            if (_serviceTime == 0) return "Unknown";
            var rate = _connections / (double)_serviceTime;
            return (rate * 100).ToString();
        }

        private async void Listen()
        {
            Listener.Start();
            var semaphore = new Semaphore(64, 64);
            while (Listening)
            {
                semaphore.WaitOne();
                Socket client;
                try
                {
                    // block until client connects
                    semaphore.Release();
                    client = await Listener.AcceptSocketAsync();
                }
                catch (Exception e) // Exception thrown when socket is closed in Stop()
                { break; }
                // pass control to ConnectionHandler.Handle
                var handler = new ConnectionHandler(this, client);
                new Thread(handler.Handle).Start();
            }
            Listener.Stop();
        }

        private void Start(int port)
        {
            Listening = true;
            Listener = new TcpListener(IPAddress.Any, port);
            Listen();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            Listening = false;
            Listener.Server.Close();
        }

        public void IncrementStatistics(DateTime start)
        {
            Interlocked.Add(ref _connections, 1);
            Interlocked.Add(ref _serviceTime, (Int64)(DateTime.Now - start).TotalMilliseconds);
        }
    }
}

