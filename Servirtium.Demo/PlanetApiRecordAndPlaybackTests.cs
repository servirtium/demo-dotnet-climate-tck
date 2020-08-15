using Microsoft.Extensions.Logging;
using Servirtium.AspNetCore;
using Servirtium.Core;
using Servirtium.Core.Http;
using Servirtium.Core.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Servirtium.Core.Interactions.FindAndReplaceScriptWriter;

namespace Servirtium.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class PlanetApiRecordAndPlaybackTests : PlanetApiTests
    {
        internal override IEnumerable<(IServirtiumServer, PlanetApi)> GenerateTestServerClientPairs(string script)
        {
            var loggerFactory = LoggerFactory.Create((builder) => builder
                .AddConsole()
                .AddDebug());
            var recorder = new InteractionRecorder(
                PlanetApi.DEFAULT_SITE, $@"..\..\..\test_recording_output\{script}".Replace("\\", ""+System.IO.Path.DirectorySeparatorChar),
                new FindAndReplaceScriptWriter(new[] {
                    new RegexReplacement(new Regex("User-Agent: .*"), "User-Agent: Servirtium-Testing")
                }, new MarkdownScriptWriter(null, loggerFactory), loggerFactory), loggerFactory);
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    recorder,
                    new SimpleHttpMessageTransforms(
                        PlanetApi.DEFAULT_SITE,
                        new Regex[0],
                        new[] {
                        "Date:", "X-", "Strict-Transport-Security",
                        "Content-Security-Policy", "Cache-Control", "Secure", "HttpOnly",
                        "Set-Cookie: climatedata.cookie=" }.Select(pattern => new Regex(pattern)),
                        loggerFactory
                    ), loggerFactory),
                new PlanetApi(new Uri("http://localhost:1234"))
            );
            var replayer = new InteractionReplayer(null, null, null, null, loggerFactory);
            replayer.LoadScriptFile($@"..\..\..\test_recording_output\{script}".Replace("\\", ""+System.IO.Path.DirectorySeparatorChar));
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    replayer,
                    new SimpleHttpMessageTransforms(
                        PlanetApi.DEFAULT_SITE,
                        new Regex[0],
                        new[] { new Regex("Date:") },
                        loggerFactory
                    ), loggerFactory),
                new PlanetApi(new Uri("http://localhost:1234"))
            );
        }
    }
}
