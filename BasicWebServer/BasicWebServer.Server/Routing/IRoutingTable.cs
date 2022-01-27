namespace BasicWebServer.Server.Routing
{
    using BasicWebServer.Server.Http;

    public interface IRoutingTable
    {
        IRoutingTable Map(Method method, string path, Func<Request, Response> responseFunction);
    }
}
