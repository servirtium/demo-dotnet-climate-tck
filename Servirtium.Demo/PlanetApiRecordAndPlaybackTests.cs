using Servirtium.AspNetCore;
using Servirtium.Core;
using Servirtium.Core.Record;
using Servirtium.Core.Replay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Servirtium.Core.FindAndReplaceScriptWriter;

namespace Servirtium.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class PlanetApiRecordAndPlaybackTests : PlanetApiTests
    {
        internal override IEnumerable<(IServirtiumServer, PlanetApi)> GenerateTestServerClientPairs(string script)
        {
            var recorder = new InteractionRecorder(
                PlanetApi.DEFAULT_SITE, $@"..\..\..\test_recording_output\{script}",
                new FindAndReplaceScriptWriter(new[] {
                    new RegexReplacement(new Regex("User-Agent: .*"), "User-Agent: Servirtium-Testing")
                }, new MarkdownScriptWriter()));
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    recorder,
                    new SimpleInteractionTransforms(
                        PlanetApi.DEFAULT_SITE,
                        new Regex[0],
                        new[] {
                        "Date:", "X-", "Strict-Transport-Security",
                        "Content-Security-Policy", "Cache-Control", "Secure", "HttpOnly",
                        "Set-Cookie: climatedata.cookie=" }.Select(pattern => new Regex(pattern))
                    )),
                new PlanetApi(new Uri("http://localhost:1234"))
            );
            var replayer = new InteractionReplayer();
            replayer.LoadScriptFile($@"..\..\..\test_recording_output\{script}");
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    replayer,
                    new SimpleInteractionTransforms(
                        PlanetApi.DEFAULT_SITE,
                        new Regex[0],
                        new[] { new Regex("Date:") }
                    )),
                new PlanetApi(new Uri("http://localhost:1234"))
            );
        }
    }
}
