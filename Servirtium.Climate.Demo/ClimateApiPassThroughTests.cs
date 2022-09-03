using Servirtium.AspNetCore;
using Servirtium.Core;
using Servirtium.Core.Interactions;
using System;
using System.Collections.Generic;

namespace Servirtium.Climate.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class ClimateApiPassThroughTests : ClimateApiTests
    {
        internal override IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script)
        {
            yield return
            (
                AspNetCoreServirtiumServer.Default(1234, new PassThroughInteractionMonitor(ClimateApi.DEFAULT_SITE), ClimateApi.DEFAULT_SITE),
                new ClimateApi(new Uri("http://localhost:1234"))
            );
        }
    }
}
