using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Web_Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            
            WebServer webServer = new(44339);
            webServer.Start();
            

            webServer.AddPage(null, "<h1>text</h1>" + Environment.NewLine + "<p>hello</p>");
            webServer.AddPage("index", "<h1>(website)</h1>");
            webServer.AddPage("about/van", "<p>iVan Temnoholm</p>");
            webServer.AddPage("redirectpage", "<a href=https://logodix.com/aspnet>Visit</a>");
            webServer.AddPage("video", "<!DOCTYPE html> <html> <body> <iframe width=\"640\"height=\"480\" src=\"https://www.youtube.com/embed/tgbNymZ7vqY\"> </iframe> </body> </html>");
            WebServer.WebApplicationRunner.Run();

         
        }
    }
}
