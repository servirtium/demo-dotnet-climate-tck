using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Servirtium.Demo.PlanetService
{
    [ApiController]
    [Route("/")]
    public class PlanetController : ControllerBase
    {
        private readonly PlanetCatalogue _planetCatalogue;

        public PlanetController(PlanetCatalogue catelogue)
        {
            _planetCatalogue = catelogue;
        }

        [HttpGet("{star}")]
        [Produces("application/json")]
        public ActionResult<IEnumerable<string>> Get(string star) {
            try
            {
                return Ok(_planetCatalogue.LookupStarSystem(star).Keys);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpGet("{star}/{planet}")]
        [Produces("application/json")]
        public ActionResult<IEnumerable<string>> Get(string star, string planet)
        {
            try
            {
                return Ok(_planetCatalogue.LookupPlanet(star, planet));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpPost("{starName}/{planetName}")]
        [Produces("text/plain")]
        public ActionResult<string> Post(string starName, string planetName, [FromBody] Dictionary<string, string> planetData)
        {
            try
            {
                _planetCatalogue.RegisterPlanet(starName, planetName, planetData);
                return Ok($"Congratulations on discovering planet '{planetName}' orbiting {starName}.{Environment.NewLine}{String.Join(Environment.NewLine, planetData.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{starName}/{planetName}")]
        [Produces("text/plain")]
        public ActionResult<string> Put(string starName, string planetName, [FromBody()] Dictionary<string, string> planetData)
        {
            try
            {
                var oldData = _planetCatalogue.LookupPlanet(starName, planetName);
                _planetCatalogue.UpdatePlanet(starName, planetName, planetData);
                return Ok($@"Updating '{planetName}' orbiting {starName}. 
Old data:
{String.Join(Environment.NewLine, oldData.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}

New data:
{String.Join(Environment.NewLine, planetData.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}
");


            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
            

        [HttpDelete("{starName}/{planetName}")]
        [Produces("text/plain")]
        public ActionResult<string> Delete(string starName, string planetName)
        {
            try 
            {
                _planetCatalogue.DeletePlanet(starName, planetName);
                return Ok($"Request acknowledged. Death Star dispatched to '{planetName}' orbiting {starName}, it will be deleted shortly");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
            
    }
}
