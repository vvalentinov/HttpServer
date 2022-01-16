﻿namespace BasicWebServer.Server.Http
{
    using System;
    using System.Text;

    public class Response
    {
        public Response(StatusCode statusCode)
        {
            this.StatusCode = statusCode;

            this.Headers = new HeaderCollection();

            this.Headers.Add(Header.Server, "My Web Server");
            this.Headers.Add(Header.Date, $"{DateTime.UtcNow:r}");
        }

        public StatusCode StatusCode { get; init; }

        public HeaderCollection Headers { get;}

        public string Body { get; set; }

        public Action<Request, Response> PreRenderAction { get;protected set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

            foreach (Header header in this.Headers)
            {
                result.AppendLine(header.ToString());
            }

            result.AppendLine();

            if (string.IsNullOrEmpty(this.Body) == false)
            {
                result.Append(this.Body);
            }

            return result.ToString();
        }
    }
}
