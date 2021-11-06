using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
}
