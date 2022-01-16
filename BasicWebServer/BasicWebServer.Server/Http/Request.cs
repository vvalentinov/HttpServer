﻿namespace BasicWebServer.Server.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Request
    {
        public Method Method { get; private set; }

        public string Url { get; private set; }

        public HeaderCollection Headers { get; private set; }

        public string Body { get; private set; }

        public static Request Parse(string request)
        {
            string[] lines = request.Split("\r\n");

            string[] startLine = lines.First().Split(" ");

            Method method = ParseMethod(startLine[0]);

            string url = startLine[1];

            HeaderCollection headers = ParseHeaders(lines.Skip(1));

            string[] bodyLines = lines.Skip(headers.Count + 2).ToArray();

            string body = string.Join("\r\n", bodyLines);

            return new Request
            {
                Method = method,
                Url = url,
                Headers = headers,
                Body = body,
            };
        }

        private static HeaderCollection ParseHeaders(IEnumerable<string> headerLines)
        {
            HeaderCollection headerCollection = new HeaderCollection();

            foreach (string headerLine in headerLines)
            {
                if (headerLine == string.Empty)
                {
                    break;
                }

                string[] headerParts = headerLine.Split(":", 2);

                if (headerParts.Length != 2)
                {
                    throw new InvalidOperationException("Request is not valid.");
                }

                string headerName = headerParts[0];
                string headerValue = headerParts[1].Trim();

                headerCollection.Add(headerName, headerValue);
            }

            return headerCollection;
        }

        private static Method ParseMethod(string inputMethod)
        {
            bool isParsed = Enum.TryParse(inputMethod, out Method method);

            if (isParsed == false)
            {
                throw new InvalidOperationException($"Method '{method}' is not supported");
            }

            return method;
        }
    }
}
