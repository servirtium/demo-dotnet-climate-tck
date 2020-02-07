using Microsoft.Extensions.Hosting;
using Servirtium.Core;
using Servirtium.Demo.PlanetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Xunit;

namespace Servirtium.Demo
{
    public abstract class PlanetApiTests : IDisposable
    {
        private readonly IHost _service = PlanetServiceFactory.Create(1001);
        public PlanetApiTests()
        {
            _service.Start();
        }

        internal abstract IEnumerable<(IServirtiumServer, PlanetApi)> GenerateTestServerClientPairs(string script);


        private void RunTest(string script, Action<PlanetApi> verification)
        {
            //NOT thread safe :-P
            lock (_service)
            {
                foreach ((IServirtiumServer server, PlanetApi api) in GenerateTestServerClientPairs(script))
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
        }

        [Fact]
        public virtual void SolPlanetsExist()
        {
            RunTest
            (
                "getSolPlanets.md",
                (api) =>
                {
                    var solPlanets = api.GetPlanets("sol").Result;
                    Assert.Equal(8, solPlanets.Count());
                    Assert.Subset(new HashSet<string> { "mercury", "venus", "earth", "mars", "jupiter", "saturn", "uranus", "neptune" }, new HashSet<string>(solPlanets));
                }
            );
        }

        [Fact]
        public virtual void RegisterAPlanetAndCheckItExists()
        {
            RunTest
            (
                "registerAPlanetAndCheckItExist.md",
                (api) =>
                {
                    api.RegisterNewPlanet("sol", "yuggoth", new Dictionary<string, string> { { "moons", "4" } }).Wait();
                    var solPlanets = api.GetPlanets("sol").Result;
                    Assert.Equal(9, solPlanets.Count());
                    Assert.Subset(new HashSet<string> { "mercury", "venus", "earth", "mars", "jupiter", "saturn", "uranus", "neptune", "yuggoth" }, new HashSet<string>(solPlanets));
                }
            );
        }

        [Fact]
        public virtual void DestroyAPlanetAndCheckItDoesentExist()
        {
            RunTest
            (
                "destroyAPlanetAndCheckItHasGone.md",
                (api) =>
                {
                    api.DestroyPlanet("sol", "jupiter").Wait();
                    var solPlanets = api.GetPlanets("sol").Result;
                    Assert.Equal(7, solPlanets.Count());
                    Assert.Subset(new HashSet<string> { "mercury", "venus", "earth", "mars", "saturn", "uranus", "neptune" }, new HashSet<string>(solPlanets));
                }
            );
        }

        [Fact]
        public virtual void UpdateAPlanetAndCheckItHasCorrectData()
        {
            RunTest
            (
                "updateAPlanetAndCheckIt.md",
                (api) =>
                {
                    api.UpdatePlanet("sol", "jupiter", new Dictionary<string, string> { { "moons", "68" }, { "colour", "brown" } }).Wait();
                    var jupiterData = api.GetPlanet("sol", "jupiter").Result;
                    Assert.Equal(2, jupiterData.Count());
                    Assert.Equal("68", jupiterData["moons"]);
                    Assert.Equal("brown", jupiterData["colour"]);
                 }
            );
        }

        [Fact]
        public virtual void DestroyAPlanetThatDoesentExistReturnsNotFound()
        {
            RunTest
            (
                "destroyAPlanetThatDoesentExistReturnsNotFound.md",
                (api) =>
                {
                    var exception = Assert.ThrowsAny<AggregateException>(()=>api.DestroyPlanet("kepler-442", "kepler-442b").Wait());
                    Assert.IsType<HttpRequestException>(exception.InnerExceptions[0]); 
                    Assert.Matches($@"^DELETE Request to http://(.+)/kepler-442/kepler-442b failed, status {HttpStatusCode.NotFound}", exception.InnerExceptions[0].Message);

                }
            );
        }

        [Fact]
        public virtual void RegisterAPlanetThatAlreadyExistsIsBadRequest()
        {
            RunTest
            (
                "registerAPlanetThatAlreadyExistsIsBadRequest.md",
                (api) =>
                {
                    var exception = Assert.Throws<AggregateException>(() => api.RegisterNewPlanet("sol", "neptune", new Dictionary<string, string> { { "moons", "4" } }).Wait());
                    Assert.IsType<HttpRequestException>(exception.InnerExceptions[0]);
                    Assert.Matches($@"^POST Request to http://(.+)/sol/neptune failed, status {HttpStatusCode.BadRequest}", exception.InnerExceptions[0].Message);

                }
            );
        }


        public void Dispose()
        {
            _service.StopAsync().Wait();
        }
    }
}
