﻿namespace BasicWebServer.Server.Http
{
    using System.Collections;
    using System.Collections.Generic;

    public class CookieCollection : IEnumerable<Cookie>
    {
        private readonly Dictionary<string, Cookie> cookies;

        public CookieCollection()
        {
            this.cookies = new Dictionary<string, Cookie>();
        }

        public string this[string name]
            => this.cookies[name].Value;

        public void Add(string name, string value)
        {
            this.cookies[name] = new Cookie(name, value);
        }

        public bool Contains(string name)
        {
            return this.cookies.ContainsKey(name);
        }

        public IEnumerator<Cookie> GetEnumerator()
        {
            return this.cookies.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
