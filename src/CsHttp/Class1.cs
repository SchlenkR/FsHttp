using System;
using System.Net;
using FsHttp;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using static FsHttp.Domain;
using static FsHttp.DslCE;

#if NETSTANDARD2_0 || NETSTANDARD2_1
using UnifiedHandler = System.Net.Http.HttpClientHandler;

#else
using UnifiedHandler = System.Net.Http.SocketsHttpHandler;
#endif

namespace CsHttp;

public record HttpRequest
{
    internal string Method { get; private set; }
    internal string Url { get; private set; }

    public string GET
    {
        set
        {
            Method = "GET";
            Url = value;
        }
    }

    public IToRequest Configure(Func<Config, Config> transformer)
    {
        // var config = transformer(Defaults.defaultConfig);
        var ctx = Dsl.Http.get("");
        Dsl.Config.update<HeaderContext>(FuncConvert.ToFSharpFunc((Config cfg) => transformer(cfg)), ctx);

        return ctx;
    }
}

public static class HttpRequestExtensions
{
    public static HttpRequest Http => new();
}

public static class Test
{
    public static void Main()
    {
        var request =
            HttpRequestExtensions.Http with { GET = "https://example.com" };
    }
}

public static class ConfigExtensions
{
    // TODO: Remove FsHttp.CSharp

    public static Config IgnoreCertIssues(this Config config) =>
        Dsl.Config.With.ignoreCertIssues(config);

    public static Config Timeout(this Config config, TimeSpan value) =>
        Dsl.Config.With.timeout(value, config);

    public static Config TimeoutInSeconds(this Config config, int value) =>
        Dsl.Config.With.timeoutInSeconds(value, config);

    public static Config SetHttpClientFactory(
        this Config config,
        Func<Config, HttpClient> httpClientFactory) =>
        Dsl.Config.With.setHttpClientFactory(FuncConvert.FromFunc(httpClientFactory), config);

    public static Config TransformHttpClient(this Config config, Func<HttpClient, HttpClient> transformer) =>
        Dsl.Config.With.transformHttpClient(FuncConvert.FromFunc(transformer), config);

    public static Config TransformHttpRequestMessage(
        this Config config,
        Func<HttpRequestMessage, HttpRequestMessage> transformer) =>
        Dsl.Config.With.transformHttpRequestMessage(FuncConvert.FromFunc(transformer), config);

    public static Config TransformHttpClientHandler(
        this Config config,
        Func<UnifiedHandler, UnifiedHandler> transformer) =>
        Dsl.Config.With.transformHttpClientHandler(FuncConvert.FromFunc(transformer), config);

    public static Config Proxy(this Config config, string url) =>
        Dsl.Config.With.proxy(url, config);

    public static Config ProxyWithCredentials(this Config config, string url, ICredentials credentials) =>
        Dsl.Config.With.proxyWithCredentials(url, credentials, config);

    public static Config DecompressionMethods(
        this Config config,
        IReadOnlyCollection<DecompressionMethods> decompressionMethods) =>
        Dsl.Config.With.decompressionMethods(SeqModule.ToList(decompressionMethods), config);

    public static Config NoDecompression(this Config config) =>
        Dsl.Config.With.noDecompression(config);

    public static Config CancellationToken(this Config config, CancellationToken cancellationToken) =>
        Dsl.Config.With.cancellationToken(cancellationToken, config);
}
