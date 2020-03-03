using Servirtium.AspNetCore;
using Servirtium.Core;
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
            var recorder = new InteractionRecorder($@"proxy_test_recording_output\{script}",
                new FindAndReplaceScriptWriter(new[] {
                    new FindAndReplaceScriptWriter.RegexReplacement(new Regex("User-Agent: .*"), "User-Agent: Servirtium-Testing")
                }, new MarkdownScriptWriter()), true);
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    recorder,
                    new SimpleInteractionTransforms(
                        new Regex[0],
                        new[] {
                            "Date:", "X-", "Strict-Transport-Security",
                            "Content-Security-Policy", "Cache-Control", "Secure", "HttpOnly",
                            "Set-Cookie: climatedata.cookie=" }.Select(pattern => new Regex(pattern))
                    )),
                new PlanetApi(_client)
            );
            var replayer = new InteractionReplayer();
            replayer.LoadScriptFile($@"proxy_test_recording_output\{script}");
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    replayer,
                    new SimpleInteractionTransforms(
                        new Regex[0],
                        new[] { new Regex("Date:") }
                    )),
                new PlanetApi(_client)
            );
        }
    }
}
