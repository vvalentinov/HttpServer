namespace BasicWebServer.Demo
{
    using BasicWebServer.Server;
    using BasicWebServer.Server.Http;
    using BasicWebServer.Server.Responses;
    using System.Text;
    using System.Web;

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
                               .MapPost("/Content", new TextFileResponse(Startup.FileName))
                               .MapGet("/Cookies", new HtmlResponse("", Startup.AddCookiesAction))
                               .MapGet("/Session", new TextResponse("", Startup.DisplaySessionInfoAction)));

            await server.Start();
        }

        private static void DisplaySessionInfoAction(Request request, Response response)
        {
            bool sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);

            if (sessionExists)
            {
                string currentDate = request.Session[Session.SessionCurrentDateKey];

                response.Body = $"Stored date: {currentDate}!";
            }
            else
            {
                response.Body = "Current date stored!";
            }
        }

        private static void AddCookiesAction(Request request, Response response)
        {
            bool requestHasCookies = request.Cookies.Any(c=> c.Name != Session.SessionCookieName);

            if (requestHasCookies)
            {
                StringBuilder cookieText = new StringBuilder();
                cookieText.AppendLine("<h1>Cookies</h1>");

                cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (Cookie cookie in request.Cookies)
                {
                    cookieText.Append("<tr>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieText.Append("</tr>");
                }

                cookieText.Append("</table>");
                response.Body = cookieText.ToString();
            }
            else
            {
                response.Body = "<h1>Cookies set!</h1>";
                response.Cookies.Add("My-Cookie", "My-Value");
                response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
            }
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