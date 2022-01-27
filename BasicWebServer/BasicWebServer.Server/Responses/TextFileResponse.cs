namespace BasicWebServer.Server.Responses
{
    using BasicWebServer.Server.Http;

    public class TextFileResponse : Response
    {
        public TextFileResponse(string fileName)
            : base(StatusCode.OK)
        {
            this.FileName = fileName;

            this.Headers.Add(Header.ContentType, ContentType.PlainText);
        }

        public string FileName { get; init; }

        public override string ToString()
        {
            if (File.Exists(this.FileName))
            {
                this.Body = string.Empty;
                this.FileContent = File.ReadAllBytes(this.FileName);

                long fileBytesCount = new FileInfo(this.FileName).Length;

                this.Headers.Add(Header.ContentLength, fileBytesCount.ToString());

                this.Headers.Add(Header.ContentDisposition, $"attachment; filename=\"{this.FileName}\"");
            }

            return base.ToString();
        }
    }
}
