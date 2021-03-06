﻿using Microsoft.Extensions.Logging;
using Servirtium.AspNetCore;
using Servirtium.Core;
using Servirtium.Core.Http;
using Servirtium.Core.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static Servirtium.Demo.TestDirectories;

namespace Servirtium.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class ClimateApiPlaybackTests : ClimateApiTests
    {
        internal override IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script)
        {
            var replayer = new InteractionReplayer(null, null, null, null, loggerFactory);
            replayer.LoadScriptFile(Path.Combine(PREGENERATED_PLAYBACKS_DIRECTORY,script));
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    1234,
                    replayer, 
                    new SimpleHttpMessageTransforms(
                        ClimateApi.DEFAULT_SITE, 
                        new[] { new Regex("Cookie:") }, 
                        new[] { new Regex("Date:"), new Regex("Cookie:") }, 
                        loggerFactory
                    ), loggerFactory),
                new ClimateApi(new Uri("http://localhost:1234"))
            );
        }
    }
}
