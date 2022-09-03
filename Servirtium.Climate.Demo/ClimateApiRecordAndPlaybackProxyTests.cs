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
using static Servirtium.Climate.Demo.TestDirectories;

namespace Servirtium.Climate.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class ClimateApiRecordAndPlaybackProxyTests : ClimateApiTests, IDisposable
    {
        private readonly HttpClient _client;
        public ClimateApiRecordAndPlaybackProxyTests()
        {
            Directory.CreateDirectory("proxy_test_recording_output");
            _client = new HttpClient(new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(new Uri("http://localhost:1234"), false) });
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        internal override IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script)
        {
            var targetScriptPath = Path.Combine(PROXY_RECORDING_OUTPUT_DIRECTORY, script);

            var recorder = new InteractionRecorder(targetScriptPath,
                new FindAndReplaceScriptWriter(new[] {
                    new RegexReplacement(new Regex("Set-Cookie: AWSALB=.*"), "Set-Cookie: AWSALB=REPLACED-IN-RECORDING; Expires=Thu, 15 Jan 2099 11:11:11 GMT; Path=/"),
                    new RegexReplacement(new Regex("Set-Cookie: TS0137860d=.*"), "Set-Cookie: TS0137860d=ALSO-REPLACED-IN-RECORDING; Path=/"),
                    new RegexReplacement(new Regex("Set-Cookie: TS01c35ec3=.*"), "Set-Cookie: TS01c35ec3=ONE-MORE-REPLACED-IN-RECORDING; Path=/"),
                    new RegexReplacement(new Regex("Set-Cookie: climatedataapi.cookie=.*"), "Set-Cookie: climatedataapi.cookie=1234567899999; Path=/"),
                    new RegexReplacement(new Regex("Set-Cookie: climatedataapi_ext.cookie=.*"), "Set-Cookie: climatedataapi_ext.cookie=9876543211111; Path=/"),
                    new RegexReplacement(new Regex("User-Agent: .*"), "User-Agent: Servirtium-Testing")
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
                new ClimateApi(_client)
            ); 
            var replayer = new InteractionReplayer(null, null, null, null, loggerFactory);
            replayer.LoadScriptFile(targetScriptPath);
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    replayer,
                    new SimpleHttpMessageTransforms(
                        new[] { new Regex("Cookie:") },
                        new[] { new Regex("Date:"), new Regex("Cookie:") }, loggerFactory
                    ), loggerFactory),
                new ClimateApi(_client)
            );
        }


    }
}
