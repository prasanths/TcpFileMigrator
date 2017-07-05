using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpServerConnector
{
    public delegate void DisconnectedEventHandler(object sender, ClientEventArgs e);

    public delegate void DataReceivedEventHandler(object sender, ClientEventArgs e);

    public class ClientEventArgs : EventArgs
    {
        private Socket socket;
         /// <summary>
           /// The ip address of remote client.
           /// </summary>
           public IPAddress IP
           {
               get { return ((IPEndPoint)this.socket.RemoteEndPoint ).Address; }
           }
           /// <summary>
           /// The port of remote client.
           /// </summary>
           public int Port
           {
               get{return ((IPEndPoint)this.socket.RemoteEndPoint).Port; }
           }
           /// <summary>
           /// Creates an instance of ClientEventArgs class.
           /// </summary>
           /// <param name="clientManagerSocket">The socket of server side socket that comunicates with the remote client.</param>
           public ClientEventArgs(Socket clientManagerSocket)
           {
               this.socket = clientManagerSocket;
           }
    }
}
