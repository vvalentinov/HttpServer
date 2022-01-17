namespace BasicWebServer.Demo
{
    using BasicWebServer.Server;
    using BasicWebServer.Server.Http;
    using BasicWebServer.Server.Responses;

    public class Startup
    {
        private const string HtmlForm = @"<form action='/HTML' method='POST'>
   Name: <input type='text' name='Name'/>
   Age: <input type='number' name ='Age'/>
<input type='submit' value ='Save' />
</form>";

        private const string DownloadForm = @"<form action='/Content' method='POST'>
   <input type='submit' value ='Download Sites Content' /> 
</form>";

        private const string FileName = "content.txt";


        public static async Task Main()
        {
            await DownloadSitesAsTextFile(Startup.FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" });

            HttpServer server = new HttpServer(routes => routes
                               .MapGet("/", new TextResponse("Hello from the server!"))
                               .MapGet("/HTML", new HtmlResponse(HtmlForm))
                               .MapPost("/HTML", new TextResponse("", Startup.AddFormDataAction))
                               .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
                               .MapGet("/Content", new HtmlResponse(Startup.DownloadForm))
                               .MapPost("/Content", new TextFileResponse(Startup.FileName)));

            await server.Start();
        }


        private static void AddFormDataAction(
            Request request,
            Response response)
        {
            response.Body = "";

            foreach (var (key, value) in request.Form)
            {
                response.Body += $"{key} - {value}";
                response.Body += Environment.NewLine;
            }
        }

        private static async Task<string> DownloadWebSiteContent(string url)
        {
            HttpClient httpClient = new HttpClient();

            using (httpClient)
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);

                string html = await response.Content.ReadAsStringAsync();

                return html.Substring(0, 2000);
            }
        }

        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            List<Task<string>> downloads = new List<Task<string>>();

            foreach (string url in urls)
            {
                downloads.Add(DownloadWebSiteContent(url));
            }

            string[] responses = await Task.WhenAll(downloads);

            string responsesString = string.Join(Environment.NewLine + new String('-', 100), responses);

            await File.WriteAllTextAsync(fileName, responsesString);
        }
    }
}