using System;
using System.Collections.Generic;
using System.Text;

namespace Servirtium.Demo.PlanetService
{
    public class PlanetCatalogue
    {
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _planets = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>{
                {
                    "sol", new Dictionary<string, Dictionary<string, string>>
                    {
                        {"mercury", new Dictionary<string, string>{{"moons", "0"} } },
                        {"venus", new Dictionary<string, string>{{"moons", "0"} } },
                        {"earth", new Dictionary<string, string>{{"moons", "1"} } },
                        {"mars", new Dictionary<string, string>{{"moons", "2"} } },
                        {"jupiter", new Dictionary<string, string>{{"moons", "67"} } },
                        {"saturn", new Dictionary<string, string>{{"moons", "62"} } },
                        {"uranus", new Dictionary<string, string>{{"moons", "27"} } },
                        {"neptune", new Dictionary<string, string>{{"moons", "14"} } }
                    }
                }
            };

        public Dictionary<string, Dictionary<string, string>> LookupStarSystem(string star) => _planets[star];
        public Dictionary<string, string> LookupPlanet(string star, string planet) => _planets[star][planet];

        public void RegisterPlanet(string starName, string planetName, Dictionary<string, string> planetData)
        {
            if (!_planets.TryGetValue(starName, out var planets))
            {
                planets = new Dictionary<string, Dictionary<string, string>>();
                _planets.Add(starName, planets);
            }
            planets.Add(planetName, planetData);
        }

        public void UpdatePlanet(string starName, string planetName, Dictionary<string, string> planetData)=>_planets[starName][planetName] = planetData;

        public void DeletePlanet(string starName, string planetName)=> _planets[starName].Remove(planetName);
    }
}
