﻿namespace BasicWebServer.Server.Http
{
    using BasicWebServer.Server.Common;

    public class Header
    {
        public Header(string name, string value)
        {
            Guard.AgainstNull(name, nameof(name));
            Guard.AgainstNull(value, nameof(value));

            this.Name = name;
            this.Value = value;
        }

        public string Name { get; init; }

        public string Value { get; set; }
    }
}
