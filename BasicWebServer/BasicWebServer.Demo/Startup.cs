namespace BasicWebServer.Demo
{
    using BasicWebServer.Server;

    public class Startup
    {
        public static void Main()
        {
            HttpServer server = new HttpServer("127.0.0.1", 8080);
            server.Start();
        }
    }
}