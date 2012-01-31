using System.Web;

namespace Xyzzy.Web
{
    public class HealthHttpHandler : IHttpHandler
    {
        public bool IsReusable {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context) {
            context.Response.ContentType = "text/html";
            context.Response.Write("I'm alive! + [" + context.Request.RawUrl + "]<br/>");
            context.Response.StatusCode = 200;

            foreach(var item in HealthHttpModule.SlowRequests) {
                context.Response.Write(string.Format("{0} {1} {2}<br/>", item.ArrivedAt.ToString("yyyy-mm-dd hh:MM:ss"), item.Url, item.Duration));
            }
        }
    }
}
