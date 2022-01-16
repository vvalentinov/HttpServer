namespace BasicWebServer.Server.Responses
{
    using BasicWebServer.Server.Http;

    public class NotFoundResponse : Response
    {
        public NotFoundResponse()
            : base(StatusCode.NotFound)
        {
        }
    }
}
