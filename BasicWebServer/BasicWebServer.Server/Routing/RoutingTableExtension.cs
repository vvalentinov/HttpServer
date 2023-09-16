namespace BasicWebServer.Server.Routing
{
    using BasicWebServer.Server.Controllers;
    using BasicWebServer.Server.Http;
    using System;

    public static class RoutingTableExtension
    {
        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, Response> controllerFunction) where TController : Controller
        {
            return routingTable.Map(
                Method.Get,
                path,
                request => controllerFunction(CreateController<TController>(request)));
        }

        public static IRoutingTable MapPost<TController>(
           this IRoutingTable routingTable,
           string path,
           Func<TController, Response> controllerFunction) where TController : Controller
        {
            return routingTable.Map(
                Method.Post,
                path,
                request => controllerFunction(CreateController<TController>(request)));
        }

        private static TController CreateController<TController>(Request request)
            => (TController)Activator.CreateInstance(typeof(TController), new[] { request });
    }
}
