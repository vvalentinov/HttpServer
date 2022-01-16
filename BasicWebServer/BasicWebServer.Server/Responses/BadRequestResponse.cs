namespace BasicWebServer.Server.Responses
{
    using BasicWebServer.Server.Http;

    public class BadRequestResponse : Response
    {
        public BadRequestResponse()
            : base(StatusCode.BadRequest)
        {
        }
    }
}
