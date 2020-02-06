using Microsoft.Extensions.Hosting;
using Servirtium.Core;
using Servirtium.Demo.PlanetService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            foreach ((IServirtiumServer server, PlanetApi api) in GenerateTestServerClientPairs(script))
            {
                try
                {
                    server.Start();
                    verification(api);
                }
                finally
                {
                    server.Stop();
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
                    Assert.Subset(new HashSet<string> { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", }, new HashSet<string>(solPlanets));
                }
            );
        }

        public void Dispose()
        {
            _service.StopAsync().Wait();
        }
    }
}
