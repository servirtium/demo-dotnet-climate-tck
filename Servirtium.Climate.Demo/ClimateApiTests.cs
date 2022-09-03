using Servirtium.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Servirtium.Climate.Demo
{
    public abstract class ClimateApiTests
    {
        
        protected static ILoggerFactory loggerFactory = LoggerFactory.Create((builder) => builder
            .AddConsole()
            .AddDebug());
        
        internal abstract IEnumerable<(IServirtiumServer, ClimateApi)> GenerateTestServerClientPairs(string script);

        private void RunTest(string script, Action<ClimateApi> verification)
        {
            foreach ((IServirtiumServer server, ClimateApi api) in GenerateTestServerClientPairs(script))
            {
                try
                {
                    server.Start();
                    verification(api);
                }
                finally
                {
                    server.Stop().Wait();
                    
                }
            }
        }

        [Fact]
        public virtual void AverageRainfallForGreatBritainFrom1980to1999Exists()
        {
            RunTest(
                "averageRainfallForGreatBritainFrom1980to1999Exists.md", 
                (api) => Assert.Equal(988.8454972331015, api.GetAveAnnualRainfall(1980, 1999, "gbr").Result, 0)
            );
        }

        [Fact]
        public void AverageRainfallForFranceFrom1980to1999Exists()
        {
            RunTest(
               "averageRainfallForFranceFrom1980to1999Exists.md",
               (api) => Assert.Equal(913.7986955122727, api.GetAveAnnualRainfall(1980, 1999, "fra").Result, 0)
            );
        }

        [Fact]
        public void AverageRainfallForEgyptFrom1980to1999Exists()
        {
            RunTest(
               "averageRainfallForEgyptFrom1980to1999Exists.md",
               (api) => Assert.Equal(54.58587712129825, api.GetAveAnnualRainfall(1980, 1999, "egy").Result, 0)
            );
        }

        [Fact]
        public void AverageRainfallForGreatBritainFrom1985to1995DoesNotExist()
        {
            RunTest(
               "averageRainfallForGreatBritainFrom1985to1995DoesNotExist.md",
               (api) => 
               {
                   var e = Assert.Throws<AggregateException>(() => api.GetAveAnnualRainfall(1985, 1995, "gbr").Wait());
                   Assert.Equal("date range 1985-1995 not supported", e.InnerExceptions[0].Message);
               }
            );
        }

        [Fact]
        public void AverageRainfallForMiddleEarthFrom1980to1999DoesNotExist()
        {
            RunTest(
               "averageRainfallForMiddleEarthFrom1980to1999DoesNotExist.md",
               (api) =>
               {
                   var e = Assert.Throws<AggregateException>(() => api.GetAveAnnualRainfall(1980, 1999, "mde").Wait());
                   Assert.Equal("mde not recognized by climateweb", e.InnerExceptions[0].Message);
               }
            );
        }

        [Fact]
        public void AverageRainfallForGreatBritainAndFranceFrom1980to1999CanBeCalculatedFromTwoRequests()
        {
            RunTest(
               "averageRainfallForGreatBritainAndFranceFrom1980to1999CanBeCalculatedFromTwoRequests.md",
               (api) => Assert.Equal(951.3220963726872, api.GetAveAnnualRainfall(1980, 1999, "gbr", "fra").Result, 0)
            );
        }

        [Fact]
        public void AverageRainfallForNeptuneServiceNotFound()
        {
            RunTest(
               "averageRainfallForNeptuneFrom1980to1999NotFoundError.md",
               (api) => {
                   var exception = Assert.Throws<AggregateException>(() => api.GetPlanetaryRainfall(1980, 1999, "neptune").Result);
                   Assert.IsType<HttpRequestException>(exception.InnerExceptions[0]);
                   Assert.Matches($@"^GET Request to http://(.+)/climateweb/rest/v1/planet/annualavg/pr/1980/1999/neptune\.xml failed, status {HttpStatusCode.NotFound}", exception.InnerExceptions[0].Message);
               }
            );
        }
    }
}
