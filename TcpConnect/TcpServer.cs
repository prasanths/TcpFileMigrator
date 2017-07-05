using System;
using System.Net.Sockets;
using System.Net;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using CloudMigrator.Azure;
using System.IO;

namespace TcpServerConnector
{
    public class TcpServer
    {
        public IPAddress IP
          {
              get
              {
                  if ( this.socket != null)
                       return ((IPEndPoint)this.socket.RemoteEndPoint ).Address;
                   else
                       return IPAddress.None;
               }
           }
           /// <summary>
           /// Gets the port number of connected remote client.This is -1 if the client is not connected.
           /// </summary>
           public int Port
           {
               get
               {
                   if ( this.socket != null)
                       return ( (IPEndPoint)this.socket.RemoteEndPoint ).Port;
                   else
                       return -1;
               }
           }
           /// <summary>
           /// [Gets] The value that specifies the remote client is connected to this server or not.
           /// </summary>
           public bool Connected
           {
               get
               {
                   if ( this.socket != null )
                       return this.socket.Connected;
                   else
                       return false;
               }
           }
   
           private Socket socket;
           private string clientName;
           /// <summary>
           /// The name of remote client.
           /// </summary>
           public string ClientName
           {
               get { return this.clientName; }
               set { this.clientName = value; }
          }
           NetworkStream networkStream;
           private BackgroundWorker bwReceiver;

        private bool IsTaskFinished = false;
   
        public TcpServer(Socket clientSocket)
        {
            this.socket = clientSocket;
            this.networkStream = new NetworkStream(this.socket);
            this.bwReceiver = new BackgroundWorker();
            this.bwReceiver.DoWork += new DoWorkEventHandler(StartReceive);
            this.bwReceiver.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
            this.bwReceiver.RunWorkerAsync();
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.IsTaskFinished = true;
            if ((e.Cancelled == true))
            {
                Console.WriteLine("Work cancelled");
            }

            else if (!(e.Error == null))
            {
                Console.WriteLine("Error: " + e.Error.Message);
            }

            else
            {
                Console.WriteLine("Work completed");
            }
            this.OnDisconnected(new ClientEventArgs(this.socket));
            this.Disconnect();
        }

        private void StartReceive(object sender, DoWorkEventArgs e)
        {
            while (this.socket.Connected)
            {
                byte[] buffer = new byte[8192];
                string fname = DateTime.Now.ToString("yyyyMMddHHmmss");
                int readBytes = this.networkStream.Read(buffer, 0, 8192);
                if (readBytes == 0)
                    break;
                else
                {
                    Console.WriteLine($"Data received from client:->> { this.IP.ToString()}:{this.Port.ToString()}");

                    //Upload....
                    var blobSvc = new BlobMigrationService();
                    blobSvc.UploadZipFileAsync(fname, new MemoryStream(buffer));
                }
            }
        }

        /// <summary>
          /// Occurs when a client disconnected from this server.
         /// </summary>
        public event DisconnectedEventHandler Disconnected;

        public bool Disconnect()
        {
            if (this.socket != null && this.socket.Connected)
            {
                try
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
                return true;
        }

        protected virtual void OnDisconnected(ClientEventArgs e)
        {
             if (Disconnected != null )
                Disconnected(this , e);
        }


}
}
