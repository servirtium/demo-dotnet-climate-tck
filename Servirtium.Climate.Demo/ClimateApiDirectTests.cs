using Servirtium.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Servirtium.Climate.Demo
{
    [Xunit.Collection("Servirtium ClimateApi Demo")]
    [CollectionDefinition(nameof(ClimateApiDirectTests), DisableParallelization = true)]
    public class ClimateApiDirectTests : ClimateApiTests
    {
        internal override IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script)
        {
            yield return (new StubServirtiumServer(), new ClimateApi(ClimateApi.GetRealServiceUrl()));
        }
        
        [Fact]
        public override void AverageRainfallForGreatBritainFrom1980to1999Exists()
        {
            base.AverageRainfallForGreatBritainFrom1980to1999Exists();
        }

        [Fact]
        public override void AverageRainfallForFranceFrom1980to1999Exists()
        {
            base.AverageRainfallForFranceFrom1980to1999Exists();
        }

        [Fact]
        public override void AverageRainfallForEgyptFrom1980to1999Exists()
        {
            base.AverageRainfallForEgyptFrom1980to1999Exists();
        }

        [Fact]
        public override void AverageRainfallForGreatBritainFrom1985to1995DoesNotExist()
        {
            base.AverageRainfallForGreatBritainFrom1985to1995DoesNotExist();
        }

        [Fact]
        public override void AverageRainfallForMiddleEarthFrom1980to1999DoesNotExist()
        {
            base.AverageRainfallForMiddleEarthFrom1980to1999DoesNotExist();
        }

        [Fact]
        public override void AverageRainfallForGreatBritainAndFranceFrom1980to1999CanBeCalculatedFromTwoRequests()
        {
            base.AverageRainfallForGreatBritainAndFranceFrom1980to1999CanBeCalculatedFromTwoRequests();
        }

        [Fact]
        public override void AverageRainfallForNeptuneServiceNotFound()
        {
            base.AverageRainfallForNeptuneServiceNotFound();
        }
    }
}
