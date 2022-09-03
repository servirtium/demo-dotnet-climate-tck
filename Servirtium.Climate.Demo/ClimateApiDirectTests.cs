using Servirtium.Core;
using System;
using System.Collections.Generic;

namespace Servirtium.Climate.Demo
{
    [Xunit.Collection("Servirtium ClimateApi Demo")]
    public class ClimateApiDirectTests : ClimateApiTests
    {
        internal override IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script)
        {
            yield return (new StubServirtiumServer(), new ClimateApi());
        }
    }
}
