using Servirtium.AspNetCore;
using Servirtium.Core;
using Servirtium.Core.Interactions;
using System;
using System.Collections.Generic;

namespace Servirtium.Climate.Demo
{
    [Xunit.Collection("Servirtium ClimateApi Demo")]
    public class ClimateApiPassThroughTests : ClimateApiTests
    {
        internal override IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script)
        {
            yield return
            (
                AspNetCoreServirtiumServer.Default(61417, new PassThroughInteractionMonitor(ClimateApi.DEFAULT_SITE), ClimateApi.DEFAULT_SITE),
                new ClimateApi(new Uri("http://servirtium.local.gd:61417"))
            );
        }
    }
}
