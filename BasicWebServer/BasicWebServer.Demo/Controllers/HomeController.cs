namespace BasicWebServer.Demo.Controllers
{
    using BasicWebServer.Demo.Models;
    using BasicWebServer.Server.Controllers;
    using BasicWebServer.Server.Http;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    public class HomeController : Controller
    {
        private const string FileName = "content.txt";

        public HomeController(Request request)
            : base(request)
        {
        }

        public Response Index() => Text("Hello from the server!");
        public Response Redirect() => Redirect("https://softuni.org/");
        public Response Html() => View();

        public Response HtmlFormPost()
        {
            string name = this.Request.Form["Name"];
            string age = this.Request.Form["Age"];

            FormViewModel model = new FormViewModel()
            {
                Name = name,
                Age = int.Parse(age)
            };

            return View(model);
        }

        public Response Content() => View();

        public Response DownloadContent()
        {
            DownloadSitesAsTextFile(HomeController.FileName,
                new string[] { "https://judge.softuni.org/", "https://softuni.org/" })
                .Wait();

            return File(HomeController.FileName);
        }

        public Response Cookies()
        {
            if (this.Request.Cookies.Any(c => c.Name != 
            BasicWebServer.Server.Http.Session.SessionCookieName))
            {
                StringBuilder cookieText = new StringBuilder();

                cookieText.AppendLine("<h1>Cookies</h1>");

                cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (Cookie cookie in this.Request.Cookies)
                {
                    cookieText.Append("<tr>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieText.Append("</tr>");
                }
                cookieText.Append("</table>");
                return Html(cookieText.ToString());
            }

            CookieCollection cookies = new CookieCollection();
            cookies.Add("My-Cookie", "My-Value");
            cookies.Add("My-Second-Cookie", "My-Second-Value");

            return Html("<h1>Cookies set!</h1>", cookies);
        }

        public Response Session()
        {
            string currentDateKey = "CurrentDate";

            bool sessionExists = this.Request.Session.ContainsKey(currentDateKey);

            if (sessionExists)
            {
                string currentDate = this.Request.Session[currentDateKey];

                return Text($"Stored date: {currentDate}!");
            }

            return Text("Current date stored!");
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

            await System.IO.File.WriteAllTextAsync(fileName, responsesString);
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
    }
}
