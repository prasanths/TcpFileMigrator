using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientConnector
{
    public class TcpConnector
    {
        private IPAddress ipAddress;

        private int port;

        public TcpConnector()
        {
            this.ipAddress = IPAddress.None;
            this.port = 80;
        }

        public TcpConnector(IPAddress ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public async Task ConnectAsync(Stream fileStream)
        {
            TcpClient client = null;

            try
            {
                // Create socket that is connected to server on specified port  
                client = new TcpClient();
                Socket socket = client.Client;
                byte[] byteBuffer;
                await socket.ConnectAsync(this.ipAddress, this.port);
                if (socket.Connected)
                {
                    using (MemoryStream s = new MemoryStream())
                    {
                        fileStream.CopyTo(s);
                        s.Seek(0, SeekOrigin.Begin);
                        byteBuffer = s.ToArray();
                    }                    
                    socket.Send(byteBuffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
