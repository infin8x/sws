using System;
using System.Collections.Generic;
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
        public Dictionary<IPAddress, int> ConnectionCount { get; private set; }
        public List<IPAddress> Blocklist { get; private set; }
        public bool Blocking { get; set; }

        private Int64 _connections;
        private Int64 _serviceTime;

        public Server(String rootDirectory)
        {
            RootDirectory = rootDirectory;
            _connections = 0;
            _serviceTime = 0;
            ConnectionCount = new Dictionary<IPAddress, int>();
            Blocklist = new List<IPAddress>();
            ThreadPool.SetMaxThreads(500, 500);
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
            var semaphore = new Semaphore(64, 512);
            while (Listening)
            {
                semaphore.WaitOne();
                Socket client;
                try
                {
                    semaphore.Release();
                    client = await Listener.AcceptSocketAsync();
                }
                catch (Exception e) // Exception thrown when socket is closed in Stop()
                { break; }
                // pass control to ConnectionHandler.Handle
                var handler = new ConnectionHandler(this, client);
                
                ThreadPool.QueueUserWorkItem((threadContext) => handler.Handle());
            }
            Listener.Stop();
        }

        public void Start(object port)
        {
            Listening = true;
            Listener = new TcpListener(IPAddress.Any, (int)port);
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

