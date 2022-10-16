using Microsoft.Net.Http.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Web_Server
{
    public sealed class WebServer : IDisposable
    {
        private string currentUrl;
        public string CurrentUrl => currentUrl;
        public int Count => pages.Count;
        private IDictionary<string, HtmlContent> pages { get; set; }
        private RequestContext requestContext;
        internal static string hostname;
        internal static WebServer webServer { get; private set; }
        private readonly WebListener http;
        public WebServer(int port)
        {
            http = new();
            hostname = $"https://localhost:{port}";
            webServer = this;
        }
        public bool Start()
        {
            pages ??= new Dictionary<string, HtmlContent>();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Starting] : {hostname} [host]");
            http.Start();
            return http.IsListening;
        }
        public void AddPage(string name, HtmlContent html)
        {
            string _hostname = hostname + "/" + name;
            if (!http.Settings.UrlPrefixes.Contains(UrlPrefix.Create(_hostname)))
            {
                pages.Add(_hostname, html);
                http.Settings.UrlPrefixes.Add(_hostname);
                return;
            }
            try
            {
                pages.Add(_hostname + "/", html);
                http.Settings.UrlPrefixes.Add(_hostname);
            }
            catch
            {
            }
            Thread.Sleep(500);
        }
        protected async Task AddPageComponents(string html)
        {
            await WriteAsync(Encoding.Default.GetBytes(html));
        }
        private async ValueTask WriteAsync(byte[] bytes)
        {
            await requestContext.Response.Body.WriteAsync(bytes);
            requestContext.Response.Body.Flush();
        }
        private IAsyncResult WaitCompletion()
        {
            Thread.Sleep(10);
            return Task.CompletedTask;
        }
        private void ExceptionLog(Exception ex)
        {
            if (!ex.Message.Contains(".ico"))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
        internal async ValueTask Monitor()
        {
            HtmlContent _ = null;
            http.Start();
            while (true)
            {
                await Task.Run(async () =>
                 {
                     requestContext = await http.AcceptAsync();
                     currentUrl = requestContext.Request.RawUrl.Substring(1);

                     try
                     {
                         _ = pages[hostname + "/" + currentUrl];
                     }
                     catch (Exception ex)
                     {
                         ExceptionLog(ex);
                     }
                     WaitCompletion();
                     await AddPageComponents(_.ToString());
                     Console.WriteLine(requestContext.Request.ContentLength);
                     WaitCompletion();
                     requestContext.Dispose();
                 });
            }
        }
        public class WebApplicationRunner
        {
            private static Process proc { get; set; }
            public static void Run()
            {
                proc = Process.Start("cmd.exe", $"/C start chrome {hostname}");
                Thread.Sleep(1000);
                proc.Kill();
                webServer.Monitor().
                    GetAwaiter().GetResult();
            }
        }
        public void Dispose()
        {
            http.Dispose();
        }
    }
}
