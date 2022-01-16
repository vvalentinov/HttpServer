namespace BasicWebServer.Server.Http
{
    using System.Collections.Generic;
    using System.Linq;

    public class HeaderCollection
    {
        private readonly Dictionary<string, Header> headers;

        public HeaderCollection()
        {
            this.headers = new Dictionary<string, Header>();
        }

        public int Count => this.headers.Count();

        public void Add(string name, string value)
        {
            Header header = new Header(name, value);
            this.headers.Add(name, header);
        }
    }
}
