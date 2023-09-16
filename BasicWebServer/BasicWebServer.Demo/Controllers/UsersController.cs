namespace BasicWebServer.Demo.Controllers
{
    using BasicWebServer.Server.Controllers;
    using BasicWebServer.Server.Http;

    public class UsersController : Controller
    {
        private const string Username = "user";

        private const string Password = "user123";

        public UsersController(Request request)
            : base(request)
        {
        }

        public Response Login() => View();

        public Response LogInUser()
        {
            this.Request.Session.Clear();

            bool usernameMatches = this.Request.Form["Username"] == UsersController.Username;
            bool passwordMatches = this.Request.Form["Password"] == UsersController.Password;

            if (usernameMatches && passwordMatches)
            {
                if (this.Request.Session.ContainsKey(Session.SessionUserKey) == false)
                {
                    this.Request.Session[Session.SessionUserKey] = "MyUserId";

                    CookieCollection cookies = new CookieCollection();
                    cookies.Add(Session.SessionCookieName, this.Request.Session.Id);

                    return Html("<h3>Logged successfully!</h3>", cookies);
                }

                return Html("<h3>Logged successfully!</h3>");
            }

            return Redirect("/Login");
        }

        public Response Logout()
        {
            this.Request.Session.Clear();

            return Html("<h3>Logged out successfully!</h3>");
        }

        public Response GetUserData()
        {
            if (this.Request.Session.ContainsKey(Session.SessionUserKey))
            {
                return Html($"<h3>Currently logged-in user is with username '{UsersController.Username}'</h3>");
            }

            return Redirect("/Login");
        }
    }
}
