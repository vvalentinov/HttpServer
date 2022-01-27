namespace BasicWebServer.Server.Routing
{
    using BasicWebServer.Server.Common;
    using BasicWebServer.Server.Http;
    using BasicWebServer.Server.Responses;
    using System;
    using System.Collections.Generic;

    public class RoutingTable : IRoutingTable
    {
        private readonly Dictionary<Method, Dictionary<string, Func<Request, Response>>> routes;

        public RoutingTable() => this.routes = new()
        {
            [Method.Get] = new(StringComparer.InvariantCultureIgnoreCase),
            [Method.Post] = new(StringComparer.InvariantCultureIgnoreCase),
            [Method.Put] = new(StringComparer.InvariantCultureIgnoreCase),
            [Method.Delete] = new(StringComparer.InvariantCultureIgnoreCase),
        };

        public IRoutingTable Map(
            Method method,
            string path,
            Func<Request, Response> responseFunction)
        {
            Guard.AgainstNull(path, nameof(path));
            Guard.AgainstNull(responseFunction, nameof(responseFunction));

            switch (method)
            {
                case Method.Get:
                    return MapGet(path, responseFunction);
                case Method.Post:
                    return MapPost(path, responseFunction);
                case Method.Put:
                case Method.Delete:
                default:
                    throw new InvalidOperationException($"Method '{method}' is not supported.");
            }
        }

        private IRoutingTable MapGet(
            string path,
            Func<Request, Response> responseFunction)
        {
            this.routes[Method.Get][path] = responseFunction;
            return this;
        }

        private IRoutingTable MapPost(
            string path,
            Func<Request, Response> responseFunction)
        {
            this.routes[Method.Post][path] = responseFunction;
            return this;
        }

        public Response MatchRequest(Request request)
        {
            Method requestMethod = request.Method;
            string requestUrl = request.Url;

            if (this.routes.ContainsKey(requestMethod) == false ||
                this.routes[requestMethod].ContainsKey(requestUrl) == false)
            {
                return new NotFoundResponse();
            }

            var responseFunction = this.routes[requestMethod][requestUrl];

            return responseFunction(request);
        }
    }
}
