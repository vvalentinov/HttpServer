namespace BasicWebServer.Server
{
    using BasicWebServer.Server.Http;
    using BasicWebServer.Server.Routing;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;

        private readonly RoutingTable routingTable;

        public HttpServer(
            string ipAddress,
            int port,
            Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;

            this.serverListener = new TcpListener(this.ipAddress, port);

            routingTableConfiguration(this.routingTable = new RoutingTable());
        }

        public HttpServer(int port, Action<IRoutingTable> routingTable)
            : this("127.0.0.1", port, routingTable)
        {
        }

        public HttpServer(Action<IRoutingTable> routingTable)
            : this(8080, routingTable)
        {
        }

        public async Task Start()
        {
            this.serverListener.Start();

            Console.WriteLine($"Server started on port {this.port}...");
            Console.WriteLine("Listening for requests...");

            while (true)
            {
                TcpClient connection = await serverListener.AcceptTcpClientAsync();

                _ = Task.Run(async () =>
                {
                    NetworkStream networkStream = connection.GetStream();

                    string requestText = await this.ReadRequest(networkStream);

                    Console.WriteLine(requestText);

                    Request request = Request.Parse(requestText);

                    Response response = this.routingTable.MatchRequest(request);

                    if (response.PreRenderAction != null)
                    {
                        response.PreRenderAction(request, response);
                    }

                    AddSession(request, response);

                    await WriteResponse(networkStream, response);

                    connection.Close();
                });
            }
        }

        private void AddSession(Request request, Response response)
        {
            bool sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);

            if (sessionExists == false)
            {
                request.Session[Session.SessionCurrentDateKey] = DateTime.Now.ToString();
                response.Cookies.Add(Session.SessionCookieName, request.Session.Id);
            }
        }

        private async Task<string> ReadRequest(NetworkStream networkStream)
        {
            int bufferLength = 1024;
            byte[] buffer = new byte[bufferLength];

            int totalBytes = 0;

            StringBuilder requestBuilder = new StringBuilder();

            do
            {
                int bytesRead = await networkStream.ReadAsync(buffer, 0, bufferLength);

                totalBytes += bytesRead;
                if (totalBytes > 10 * bufferLength)
                {
                    throw new InvalidOperationException("Request is too large.");
                }

                requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            } while (networkStream.DataAvailable);

            return requestBuilder.ToString();
        }

        private async Task WriteResponse(NetworkStream networkStream, Response response)
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(response.ToString());

            await networkStream.WriteAsync(responseBytes);
        }
    }
}
