using Microsoft.Extensions.Logging;
using Servirtium.AspNetCore;
using Servirtium.Core;
using Servirtium.Core.Http;
using Servirtium.Core.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using static Servirtium.Climate.Demo.TestDirectories;

namespace Servirtium.Climate.Demo
{
    [Xunit.Collection("Servirtium ClimateApi Demo")]
    public class ClimateApiPlaybackTests : ClimateApiTests
    {
        internal override IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script)
        {
            var replayer = new InteractionReplayer(null, null, null, null, loggerFactory);
            replayer.LoadScriptFile(Path.Combine(PREGENERATED_PLAYBACKS_DIRECTORY,script));
            yield return
            (
                AspNetCoreServirtiumServer.WithTransforms(
                    61417,
                    replayer, 
                    new SimpleHttpMessageTransforms(
                        ClimateApi.DEFAULT_SITE, 
                        new[] { new Regex("Cookie:") }, 
                        new[] { new Regex("Date:"), new Regex("Cookie:") }, 
                        loggerFactory
                    ), loggerFactory),
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
