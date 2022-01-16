namespace BasicWebServer.Server
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;

        public HttpServer(string ipAddress, int port)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            this.serverListener = new TcpListener(this.ipAddress, port);
        }

        public void Start()
        {
            this.serverListener.Start();

            Console.WriteLine($"Server started on port {this.port}...");
            Console.WriteLine("Listening for requests...");

            while (true)
            {
                TcpClient connection = serverListener.AcceptTcpClient();

                NetworkStream networkStream = connection.GetStream();

                string requestText = this.ReadRequest(networkStream);

                Console.WriteLine(requestText);

                WriteResponse(networkStream, "Hello from the server!");

                connection.Close();
            }
        }

        private string ReadRequest(NetworkStream networkStream)
        {
            int bufferLength = 1024;
            byte[] buffer = new byte[bufferLength];

            int totalBytes = 0;

            StringBuilder requestBuilder = new StringBuilder();

            do
            {
                int bytesRead = networkStream.Read(buffer, 0, bufferLength);

                totalBytes += bytesRead;
                if (totalBytes > 10 * bufferLength)
                {
                    throw new InvalidOperationException("Request is too large.");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            } while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }

        private void WriteResponse(NetworkStream networkStream, string message)
        {
            int contentLength = Encoding.UTF8.GetByteCount(message);

            string response = $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLength}

{message}";

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            networkStream.Write(responseBytes);
        }
    }
}
