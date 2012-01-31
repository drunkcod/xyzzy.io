using System;
using System.Diagnostics;
using System.Web;

using RequestQueue = System.Collections.Generic.Queue<Xyzzy.Web.RequestStatistics>;

namespace Xyzzy.Web
{
    public class RequestStatistics 
    {
        public string Url;
        public DateTime ArrivedAt;
        public TimeSpan Duration;
    }

    public class HealthHttpModule : IHttpModule
    {
        public static int MaxRequests = 1000;
        public static TimeSpan SlowRequestThreshold = TimeSpan.FromMilliseconds(10);
        public static RequestQueue SlowRequests = new RequestQueue();

        DateTime ArrivedAt;
        Stopwatch ProcessingTime;

        public void Dispose() { }

        public void Init(HttpApplication context) {
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        void BeginRequest(object sender, EventArgs e) {
            ArrivedAt = DateTime.UtcNow;
            ProcessingTime = Stopwatch.StartNew();
        }

        void EndRequest(object sender, EventArgs e) {
            var app = (HttpApplication)sender;
            ProcessingTime.Stop();
            if(ProcessingTime.Elapsed < SlowRequestThreshold)
                return;

            var result = new RequestStatistics {
                Url = app.Request.RawUrl, 
                ArrivedAt = ArrivedAt,
                Duration = ProcessingTime.Elapsed
            };
            lock(SlowRequests) {
                SlowRequests.Enqueue(result);
                while(SlowRequests.Count > MaxRequests)
                    SlowRequests.Dequeue();
            }
        }
    }
}
