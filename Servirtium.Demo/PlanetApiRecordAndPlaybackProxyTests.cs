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
using System.Text.RegularExpressions;
using static Servirtium.Demo.TestDirectories;

namespace Servirtium.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class PlanetApiRecordAndPlaybackProxyTests : PlanetApiTests, IDisposable
    {
        private readonly HttpClient _client;
        public PlanetApiRecordAndPlaybackProxyTests()
        {
            _client = new HttpClient(new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(new Uri("http://localhost:1234"), false) });
        }

        public override void Dispose()
        {
            base.Dispose();
            _client.Dispose();
        }
        
        internal override IEnumerable<(IServirtiumServer, PlanetApi)> GenerateTestServerClientPairs(string script, IEnumerable<RegexReplacement>? transformReplacements = null)
        {
            IEnumerable<RegexReplacement> replacements = transformReplacements ?? new RegexReplacement[0];
            var targetScriptPath = Path.Combine(PROXY_RECORDING_OUTPUT_DIRECTORY, script);
            var recorder = new InteractionRecorder(targetScriptPath,
                new FindAndReplaceScriptWriter(new[] {
                    new RegexReplacement(new Regex("User-Agent: .*"), "User-Agent: Servirtium-Testing")
                }, new MarkdownScriptWriter(null, loggerFactory), loggerFactory), true, loggerFactory);
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    recorder,
                    new HttpMessageTransformPipeline(
                        new SimpleHttpMessageTransforms(
                            new Regex[0],
                            new[] {
                                "Date:", "X-", "Strict-Transport-Security",
                                "Content-Security-Policy", "Cache-Control", "Secure", "HttpOnly",
                                "Set-Cookie: climatedata.cookie=" }.Select(pattern => new Regex(pattern)), 
                            loggerFactory
                        ),
                        new FindAndReplaceHttpMessageTransforms(replacements, loggerFactory)
                    ), loggerFactory),
                new PlanetApi(_client)
            );
            var replayer = new InteractionReplayer(null, null, null, null, loggerFactory);
            replayer.LoadScriptFile(targetScriptPath);
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    replayer,
                    new HttpMessageTransformPipeline(
                        new SimpleHttpMessageTransforms(
                            new Regex[0],
                            new[] { new Regex("Date:") }, 
                            loggerFactory
                        ),
                        new FindAndReplaceHttpMessageTransforms(replacements, loggerFactory)
                    ), loggerFactory
                ),
                new PlanetApi(_client)
            );
        }
    }
}
