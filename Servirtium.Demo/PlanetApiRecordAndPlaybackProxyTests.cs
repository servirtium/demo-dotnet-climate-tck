using Microsoft.Extensions.Logging;
using Servirtium.AspNetCore;
using Servirtium.Core;
using Servirtium.Core.Http;
using Servirtium.Core.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Servirtium.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class PlanetApiRecordAndPlaybackProxyTests : PlanetApiTests
    {
        private readonly HttpClient _client;
        public PlanetApiRecordAndPlaybackProxyTests()
        {
            if (!Directory.Exists("proxy_test_recording_output"))
            {
                Directory.CreateDirectory("proxy_test_recording_output");
            }
            _client = new HttpClient(new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(new Uri("http://localhost:1234"), false) });
        }

        internal override IEnumerable<(IServirtiumServer, PlanetApi)> GenerateTestServerClientPairs(string script)
        {
            var loggerFactory = LoggerFactory.Create((builder) => builder
                .AddConsole()
                .AddDebug());
            var recorder = new InteractionRecorder($@"proxy_test_recording_output\{script}",
                new FindAndReplaceScriptWriter(new[] {
                    new FindAndReplaceScriptWriter.RegexReplacement(new Regex("User-Agent: .*"), "User-Agent: Servirtium-Testing")
                }, new MarkdownScriptWriter(null, loggerFactory), loggerFactory), true, loggerFactory);
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    recorder,
                    new SimpleHttpMessageTransforms(
                        new Regex[0],
                        new[] {
                            "Date:", "X-", "Strict-Transport-Security",
                            "Content-Security-Policy", "Cache-Control", "Secure", "HttpOnly",
                            "Set-Cookie: climatedata.cookie=" }.Select(pattern => new Regex(pattern)), 
                        loggerFactory
                    ), loggerFactory),
                new PlanetApi(_client)
            );
            var replayer = new InteractionReplayer(null, null, null, null, loggerFactory);
            replayer.LoadScriptFile($@"proxy_test_recording_output\{script}");
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    replayer,
                    new SimpleHttpMessageTransforms(
                        new Regex[0],
                        new[] { new Regex("Date:") }, 
                        loggerFactory
                    ), loggerFactory),
                new PlanetApi(_client)
            );
        }
    }
}
