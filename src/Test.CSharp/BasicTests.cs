using System;
using System.Threading.Tasks;
using FsHttp.CSharp;
using FsHttp.Tests;
using NUnit.Framework;

namespace Test.CSharp
{
    public class BasicTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public async Task PostsText()
        {
            const string Content = "Hello World";

            using var server = Server.Predefined.postReturnsBody();

            var response = await
                (await Server.url("").Post()
                    .Body()
                    .Text(Content)
                    .SendAsync())
                .ToTextAsync();

            Assert.AreEqual(Content, response);
        }

        public record Person(string Name, string Job);

        [Test]
        public async Task PostsJsonObject()
        {
            var jsonObj = new Person("morpheus", "leader");

            using var server = Server.Predefined.postReturnsBody();

            var response = await
                (await Server.url("").Post()
                    .Body()
                    .JsonSerialize(jsonObj)
                    .SendAsync())
                .DeserializeJsonAsync<Person>();

            Assert.AreEqual(jsonObj, response);
        }

        [Test]
        public void Configuration()
        {
            const string Content = "Hello World";

            using var server = Server.Predefined.postReturnsBody();

            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await
                    (await Server.url("").Post()
                        .Body()
                        .Text(Content)
                        .Configure(c => c.Timeout(TimeSpan.FromTicks(1)))
                        .SendAsync())
                    .ToTextAsync());
        }
    }
}