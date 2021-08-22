using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestWebServer.Api
{
    [Route("test")]
    public class TestController : Controller
    {
        [HttpGet("lines")]
        public IActionResult Get() =>
            new FileCallbackResult(
                new MediaTypeHeaderValue("application/octet-stream"),
                async (outputStream, _) =>
                {
                    await using var sw = new StreamWriter(outputStream);

                    for (var i = 0; i < 1000; i++)
                    {
                        await sw.WriteLineAsync("skldfj skldfjklsdj flksdjfkl sdjlkfj sdlk fjskld fjlkj");
                        await Task.Delay(10);
                    }
                })
            {
                FileDownloadName = "MyZipfile.zip"
            };
    }

    public class FileCallbackResult : FileResult
    {
        private readonly Func<Stream, ActionContext, Task> _callback;

        public FileCallbackResult(MediaTypeHeaderValue contentType, Func<Stream, ActionContext, Task> callback)
            : base(contentType?.ToString())
        {
            _callback = callback;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var executor = new FileCallbackResultExecutor(
                context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>());
            return executor.ExecuteAsync(context, this);
        }

        private sealed class FileCallbackResultExecutor : FileResultExecutorBase
        {
            public FileCallbackResultExecutor(ILoggerFactory loggerFactory)
                : base(CreateLogger<FileCallbackResultExecutor>(loggerFactory))
            {
            }

            public Task ExecuteAsync(ActionContext context, FileCallbackResult result)
            {
                SetHeadersAndLog(context, result, null, true);
                return result._callback(context.HttpContext.Response.Body, context);
            }
        }
    }
}