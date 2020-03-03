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
    public class ClimateApiRecordAndPlaybackProxyTests : ClimateApiTests
    {
        private readonly HttpClient _client;
        public ClimateApiRecordAndPlaybackProxyTests()
        {
            if (!Directory.Exists("proxy_test_recording_output"))
            {
                Directory.CreateDirectory("proxy_test_recording_output");
            }
            _client = new HttpClient(new HttpClientHandler() { UseProxy = true, Proxy = new WebProxy(new Uri("http://localhost:1234"), false) });
        }


        internal override IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script)
        {

            var recorder = new InteractionRecorder($@"proxy_test_recording_output\{script}",
                new FindAndReplaceScriptWriter(new[] {
                    new FindAndReplaceScriptWriter.RegexReplacement(new Regex("Set-Cookie: AWSALB=.*"), "Set-Cookie: AWSALB=REPLACED-IN-RECORDING; Expires=Thu, 15 Jan 2099 11:11:11 GMT; Path=/"),
                    new FindAndReplaceScriptWriter.RegexReplacement(new Regex("Set-Cookie: TS0137860d=.*"), "Set-Cookie: TS0137860d=ALSO-REPLACED-IN-RECORDING; Path=/"),
                    new FindAndReplaceScriptWriter.RegexReplacement(new Regex("Set-Cookie: TS01c35ec3=.*"), "Set-Cookie: TS01c35ec3=ONE-MORE-REPLACED-IN-RECORDING; Path=/"),
                    new FindAndReplaceScriptWriter.RegexReplacement(new Regex("Set-Cookie: climatedataapi.cookie=.*"), "Set-Cookie: climatedataapi.cookie=1234567899999; Path=/"),
                    new FindAndReplaceScriptWriter.RegexReplacement(new Regex("Set-Cookie: climatedataapi_ext.cookie=.*"), "Set-Cookie: climatedataapi_ext.cookie=9876543211111; Path=/"),
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
                new ClimateApi(_client)
            ); 
            var replayer = new InteractionReplayer();
            replayer.LoadScriptFile($@"proxy_test_recording_output\{script}");
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    replayer,
                    new SimpleInteractionTransforms(
                        new[] { new Regex("Cookie:") },
                        new[] { new Regex("Date:"), new Regex("Cookie:") }
                    )),
                new ClimateApi(_client)
            );
        }


    }
}
