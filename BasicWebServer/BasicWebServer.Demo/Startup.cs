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

        private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";


        private const string FileName = "content.txt";

        private const string Username = "user";

        private const string Password = "user123";

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
                               .MapGet("/Session", new TextResponse("", Startup.DisplaySessionInfoAction))
                               .MapGet("/Login", new HtmlResponse(Startup.LoginForm))
                               .MapPost("/Login", new HtmlResponse("", Startup.LoginAction))
                               .MapGet("/Logout", new HtmlResponse("", Startup.LogoutAction))
                               .MapGet("/UserProfile", new HtmlResponse("", Startup.GetUserDataAction)));

            await server.Start();
        }

        private static void GetUserDataAction(Request request, Response response)
        {
            if (request.Session.ContainsKey(Session.SessionUserKey))
            {
                response.Body = $"<h3>Currently logged-in user is with username '{Username}'</h3>"; 
            }
            else
            {
                response.Body = $"<h3>You should first log in - <a href='/Login'>Login</a></h3>";
            }
        }

        private static void LogoutAction(Request request, Response response)
        {
            request.Session.Clear();

            response.Body = "<h3>Logged out successfully!</h3>";
        }

        private static void LoginAction(Request request, Response response)
        {
            request.Session.Clear();

            bool usernameMatches = request.Form["Username"] == Startup.Username;
            bool passwordMatches = request.Form["Password"] == Startup.Password;

            if (usernameMatches && passwordMatches)
            {
                request.Session[Session.SessionUserKey] = "MyUserId";
                response.Cookies.Add(Session.SessionCookieName, request.Session.Id);

                response.Body = "<h3>Logged successfully!</h3>";
            }
            else
            {
                response.Body = Startup.LoginForm;
            }
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