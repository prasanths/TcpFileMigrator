using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TcpServerConnector;

namespace TcpBackgroundWorker
{
    class Program
    {
        private List<TcpServer> clientListeners;
        private BackgroundWorker bwListener;
        private Socket listenerSocket;
        private IPAddress serverIP;
        private int serverPort;

        /// <summary>
        /// Start the console server.
        /// </summary>
        /// <param name="args">These are optional arguments.Pass the local ip address of the server as the first argument and the local port as the second argument.</param>
        static void Main(string[] args)
        {
            Program program = new Program();
            program.clientListeners = new List<TcpServer>();

            if (args.Length == 0)
            {
                program.serverPort = 8080;
                program.serverIP = IPAddress.Any;
            }
            if (args.Length == 1)
            {
                program.serverIP = IPAddress.Parse(args[0]);
                program.serverPort = 8080;
            }
            if (args.Length == 2)
            {
                program.serverIP = IPAddress.Parse(args[0]);
                program.serverPort = int.Parse(args[1]);
            }

            program.bwListener = new BackgroundWorker();
            program.bwListener.WorkerSupportsCancellation = true;
            program.bwListener.DoWork += new DoWorkEventHandler(program.StartToListen);
           // progDomain.bwListener.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
            program.bwListener.RunWorkerAsync();

            Console.WriteLine($"*** Listening on port {program.serverIP.ToString()}:{program.serverPort.ToString()} started.Press ENTER to shutdown server. ***\n");

            Console.ReadLine();

            program.DisconnectServer();
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Do complete
        }

        private void StartToListen(object sender, DoWorkEventArgs e)
        {
            this.listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.listenerSocket.Bind(new IPEndPoint(this.serverIP, this.serverPort));
            this.listenerSocket.Listen(200);
            while (true)
                this.CreateClientListener(this.listenerSocket.Accept());
        }
        private void CreateClientListener(Socket socket)
        {
            TcpServer clientListener = new TcpServer(socket);
            //clientListener.DataReceived += new CommandReceivedEventHandler(DataReceived);
            clientListener.Disconnected += new DisconnectedEventHandler(ClientDisconnected);
           // this.CheckForAbnormalDC(newClientManager);
            this.clientListeners.Add(clientListener);
            this.UpdateConsole("Connected.", clientListener.IP, clientListener.Port);
        }

        private void CheckForAbnormalDC(TcpServer mngr)
        {
            if (this.RemoveClientListener(mngr.IP))
                this.UpdateConsole("Disconnected.", mngr.IP, mngr.Port);
        }

        void ClientDisconnected(object sender, ClientEventArgs e)
        {
            if (this.RemoveClientListener(e.IP))
                this.UpdateConsole("Disconnected.", e.IP, e.Port);
        }

        private bool RemoveClientListener(IPAddress ip)
        {
            lock (this)
            {
                int index = this.IndexOfClient(ip);
                if (index != -1)
                {
                    string name = this.clientListeners[index].ClientName;
                    this.clientListeners.RemoveAt(index);
                    return true;
                }
                return false;
            }
        }

        private int IndexOfClient(IPAddress ip)
        {
            int index = -1;
            foreach (TcpServer server in this.clientListeners)
            {
                index++;
                if (server.IP.Equals(ip))
                    return index;
            }
            return -1;
        }


        private void UpdateConsole(string status, IPAddress IP, int port)
        {
            Console.WriteLine($"Client {IP.ToString()}:{port.ToString()} has been {status} ( {DateTime.Now.ToString("dd / MM / yyyy")}|{DateTime.Now.ToString("HH:mm:ss")} )");
        }
        public void DisconnectServer()
        {
            if (this.clientListeners != null)
            {
                foreach (TcpServer server in this.clientListeners)
                    server.Disconnect();

                this.bwListener.CancelAsync();
                this.bwListener.Dispose();
                this.listenerSocket.Dispose();
                GC.Collect();
            }
        }
    }
}