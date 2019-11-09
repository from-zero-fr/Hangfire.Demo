using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using NLog;

namespace Hangfire.Demo.Client
{
    public class LoggingMiddleware : OwinMiddleware
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public LoggingMiddleware(OwinMiddleware next) : base(next)
        { }

        public override async Task Invoke(IOwinContext context)
        {
            LogRequestResponseHelper.LogDebugRequest(Logger, context.Request);

            var responseBody = "";
            if (Logger.IsTraceEnabled) //use trace for logging the response
            {
                using (var captureResponseBody = new CaptureResponseBody(context))
                {
                    await Next.Invoke(context);
                    responseBody = await captureResponseBody.GetBody();
                }
            }
            else
                await Next.Invoke(context);

            LogRequestResponseHelper.LogDebugResponse(Logger, context.Response);
            if (Logger.IsTraceEnabled
                && !string.IsNullOrEmpty(context.Response.ContentType) && context.Response.ContentType.ToLower().StartsWith("application/json"))
                LogRequestResponseHelper.LogTraceResponse(Logger, responseBody);
        }

        private class CaptureResponseBody : IDisposable
        {
            // Response body is a write-only network stream by default for Katana hosts. 
            // You will need to replace context.Response.Body with a MemoryStream, 
            // read the stream, log the content and then copy the memory stream content 
            // back into the original network stream

            private readonly Stream stream;
            private readonly MemoryStream buffer;

            public CaptureResponseBody(IOwinContext context)
            {
                stream = context.Response.Body;
                buffer = new MemoryStream();
                context.Response.Body = buffer;
            }

            public async Task<string> GetBody()
            {
                buffer.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(buffer);
                return await reader.ReadToEndAsync();
            }

            public async void Dispose()
            {
                await GetBody();

                // You need to do this so that the response we buffered
                // is flushed out to the client application.
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(stream);
            }
        }
    }
}
