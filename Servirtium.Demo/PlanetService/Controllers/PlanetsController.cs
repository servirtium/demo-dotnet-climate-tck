using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Servirtium.Demo.PlanetService.Controllers
{
    [ApiController]
    [Route("/planets")]
    public class PlanetsController : ControllerBase
    {
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _planetRegistry =
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                {
                    "sol", new Dictionary<string, Dictionary<string, string>>
                    {
                        {"Mercury", new Dictionary<string, string>{{"moons", "0"} } },
                        {"Venus", new Dictionary<string, string>{{"moons", "0"} } },
                        {"Earth", new Dictionary<string, string>{{"moons", "1"} } },
                        {"Mars", new Dictionary<string, string>{{"moons", "2"} } },
                        {"Jupiter", new Dictionary<string, string>{{"moons", "67"} } },
                        {"Saturn", new Dictionary<string, string>{{"moons", "62"} } },
                        {"Uranus", new Dictionary<string, string>{{"moons", "27"} } },
                        {"Neptune", new Dictionary<string, string>{{"moons", "14"} } }
                    }
                }
            };

        [HttpGet("/{star}")]
        [Produces("application/json")]
        public ActionResult<IEnumerable<string>> Get(string star) {
            try
            {
                return Ok(_planetRegistry[star].Keys);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }

        }

        [HttpPost("{starName}/{planetName}")]
        [Produces("text/plain")]
        public ActionResult<string> Post(string starName, string planetName, [FromBody()] Dictionary<string, string> planetData)
        {
            if (!_planetRegistry.TryGetValue(starName, out var planets))
            {
                planets = new Dictionary<string, Dictionary<string, string>>();
                _planetRegistry.Add(starName, planets);
            }
            try
            {
                planets.Add(planetName, planetData);
                return Ok($"Congratulations on discovering planet '{planetName}' orbiting {starName}.{Environment.NewLine}{String.Join(Environment.NewLine, planetData.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{starName}/{planetName}")]
        [Produces("text/plain")]
        public ActionResult<string> Put(string starName, string planetName, [FromBody()] Dictionary<string, string> planetData)
        {
            try
            {
                var oldData = _planetRegistry[starName][planetName];
                _planetRegistry[starName][planetName] = planetData;
                return Ok($@"Updating '{planetName}' orbiting {starName}. 
Old data:
{String.Join(Environment.NewLine, oldData.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}

New data:
{String.Join(Environment.NewLine, planetData.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}
");


            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
        }
            

        [HttpDelete("{starName}/{planetName}")]
        [Produces("text/plain")]
        public ActionResult<string> Delete(string starName, string planetName)
        {
            try 
            {
                _planetRegistry[starName].Remove(planetName);
                return Ok($"Request acknowledged. Death Star dispatched to '{planetName}' orbiting {starName}, it will be deleted shortly");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex);
            }
        }
            
    }
}
