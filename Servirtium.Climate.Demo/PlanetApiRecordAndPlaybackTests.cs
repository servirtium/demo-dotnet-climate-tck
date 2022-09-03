using Microsoft.Extensions.Logging;
using Servirtium.AspNetCore;
using Servirtium.Core;
using Servirtium.Core.Http;
using Servirtium.Core.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static Servirtium.Demo.TestDirectories;

namespace Servirtium.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class PlanetApiRecordAndPlaybackTests : PlanetApiTests
    {
        internal override IEnumerable<(IServirtiumServer, PlanetApi)> GenerateTestServerClientPairs(string script, IEnumerable<RegexReplacement>? transformReplacements = null)
        {
            IEnumerable<RegexReplacement> replacements = transformReplacements ?? new RegexReplacement[0];
            var targetScriptPath = Path.Combine(RECORDING_OUTPUT_DIRECTORY, script);
            var recorder = new InteractionRecorder(
                PlanetApi.DEFAULT_SITE, targetScriptPath,
                new FindAndReplaceScriptWriter(new[] {
                    new RegexReplacement(new Regex("User-Agent: .*"), "User-Agent: Servirtium-Testing")
                }, new MarkdownScriptWriter(null, loggerFactory), loggerFactory), loggerFactory);
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    recorder,
                    new HttpMessageTransformPipeline(
                        new SimpleHttpMessageTransforms(
                            PlanetApi.DEFAULT_SITE,
                            new Regex[0],
                            new[] {
                            "Date:", "X-", "Strict-Transport-Security",
                            "Content-Security-Policy", "Cache-Control", "Secure", "HttpOnly",
                            "Set-Cookie: climatedata.cookie=" }.Select(pattern => new Regex(pattern)),
                            loggerFactory
                        ),
                        new FindAndReplaceHttpMessageTransforms(replacements, loggerFactory)
                    ), loggerFactory),
                new PlanetApi(new Uri("http://localhost:1234"))
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
                            PlanetApi.DEFAULT_SITE,
                            new Regex[0],
                            new[] { new Regex("Date:") },
                            loggerFactory
                        ),
                        new FindAndReplaceHttpMessageTransforms(replacements, loggerFactory)
                    ), loggerFactory),
                new PlanetApi(new Uri("http://localhost:1234"))
            );
        }
    }
}
