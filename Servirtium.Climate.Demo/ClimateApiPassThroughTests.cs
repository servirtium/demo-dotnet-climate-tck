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
        public override void AverageRainfallForGreatBritainFrom1980to1999Exists()
        {
            base.AverageRainfallForGreatBritainFrom1980to1999Exists();
        }

        public override void AverageRainfallForFranceFrom1980to1999Exists()
        {
            base.AverageRainfallForFranceFrom1980to1999Exists();
        }

        public override void AverageRainfallForEgyptFrom1980to1999Exists()
        {
            base.AverageRainfallForEgyptFrom1980to1999Exists();
        }

        public override void AverageRainfallForGreatBritainFrom1985to1995DoesNotExist()
        {
            base.AverageRainfallForGreatBritainFrom1985to1995DoesNotExist();
        }

        public override void AverageRainfallForMiddleEarthFrom1980to1999DoesNotExist()
        {
            base.AverageRainfallForMiddleEarthFrom1980to1999DoesNotExist();
        }

        public override void AverageRainfallForGreatBritainAndFranceFrom1980to1999CanBeCalculatedFromTwoRequests()
        {
            base.AverageRainfallForGreatBritainAndFranceFrom1980to1999CanBeCalculatedFromTwoRequests();
        }

        public override void AverageRainfallForNeptuneServiceNotFound()
        {
            base.AverageRainfallForNeptuneServiceNotFound();
        }
    }
}
