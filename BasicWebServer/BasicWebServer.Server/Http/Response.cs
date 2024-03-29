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
            this.Cookies = new CookieCollection();

            this.Headers.Add(Header.Server, "My Web Server");
            this.Headers.Add(Header.Date, $"{DateTime.UtcNow:r}");
        }

        public StatusCode StatusCode { get; init; }

        public HeaderCollection Headers { get;}

        public CookieCollection Cookies { get;}

        public string Body { get; set; }

        public byte[] FileContent { get; set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

            foreach (Header header in this.Headers)
            {
                result.AppendLine(header.ToString());
            }

            foreach (Cookie cookie in this.Cookies)
            {
                result.AppendLine($"{Header.SetCookie}: {cookie}");
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
