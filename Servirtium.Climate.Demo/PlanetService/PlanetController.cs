using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

        [HttpGet("{star}/{planet}/photos/{photoFile}")]
        [Produces("image/png")]
        public ActionResult<Stream> Get(string star, string planet, string photoFile)
        {
            if (System.IO.File.Exists($"planet_photos/{star}/{planet}/{photoFile}"))
            {
                Directory.CreateDirectory($"planet_photos/{star}/");
                return Ok(System.IO.File.OpenRead($"planet_photos/{star}/{planet}/{photoFile}"));
            }
            else
            {
                return NotFound($"Cannot find that photo of {planet}. :-(");
            }

        }

        [HttpPost("{star}/{planet}/photos/{photoFile}")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> Post(string star, string planet, string photoFile)
        {
            try
            {
                var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);
                Bitmap bitmap = new Bitmap(ms);
                Directory.CreateDirectory($"planet_photos/{star}/{planet}");
                bitmap.Save($"planet_photos/{star}/{planet}/{photoFile}", ImageFormat.Png);
                return Ok($"Thanks for that lovely photo of {planet}! It's dimensions are {bitmap.Width}x{bitmap.Height}.");
            }
            catch(ArgumentNullException nex)
            {
                return BadRequest(nex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
