using Microsoft.Extensions.Hosting;
using Servirtium.Core;
using Servirtium.Demo.PlanetService;
using System;
using System.Collections.Generic;
using System.Linq;
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
                "destroyAPlanetAndCheckItHasGone.md",
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
                "registerAPlanetAndCheckItExist.md",
                (api) =>
                {
                    api.DestroyPlanet("sol", "jupiter").Wait();
                    var solPlanets = api.GetPlanets("sol").Result;
                    Assert.Equal(7, solPlanets.Count());
                    Assert.Subset(new HashSet<string> { "mercury", "venus", "earth", "mars", "saturn", "uranus", "neptune" }, new HashSet<string>(solPlanets));
                }
            );
        }

        public void Dispose()
        {
            _service.StopAsync().Wait();
        }
    }
}
