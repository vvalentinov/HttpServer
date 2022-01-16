namespace BasicWebServer.Server.Responses
{
    using BasicWebServer.Server.Http;

    public class UnauthorizedResponse : Response
    {
        public UnauthorizedResponse()
            : base(StatusCode.Unauthorized)
        {
        }
    }
}
