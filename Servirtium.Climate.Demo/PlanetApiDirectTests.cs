using Servirtium.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Servirtium.Demo
{
    [Xunit.Collection("Servirtium Demo")]
    public class PlanetApiDirectTests : PlanetApiTests
    {
        internal override IEnumerable<(IServirtiumServer, PlanetApi)> GenerateTestServerClientPairs(string script, IEnumerable<RegexReplacement>? transformReplacements = null)
        {
            yield return (new StubServirtiumServer(), new PlanetApi());
        }

        public override void OptionsRequestsAreIgnored()
        {
            //Not valid for direct mode
        }

        public override void UpdateAPlanetWithRequestAndResponseTransformsAndCheckThoseAreApplied()
        {
            //Not valid for direct mode
        }
    }
}
