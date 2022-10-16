namespace Web_Server
{
    public sealed class HtmlContent 
    {
        private string RawHtml { get; }
        private HtmlContent(string html) 
        {
            RawHtml = html;
        }
        public static implicit operator HtmlContent(string html) 
        {
            return new HtmlContent(html);
        }
        public override string ToString()
        {
            return RawHtml;
        }
    }
}
