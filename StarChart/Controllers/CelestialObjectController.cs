using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Remotion.Linq.Clauses;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [ApiController]
    [Route("")]
    public class CelestialObjectController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        [HttpGet("{id:int}", Name = "GetById")] 
        public IActionResult GetById(int id)
        {
            var bodies = _context.CelestialObjects.Find(id);

            if (bodies == null)
            {
                return NotFound();
            }

            bodies.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();
            return Ok(bodies);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var bodies = _context.CelestialObjects.Where(c => c.Name == name).ToList();
            if (!bodies.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in bodies)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }
            return Ok(bodies);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var bodies = _context.CelestialObjects.ToList();

            foreach (var celestialObject in  bodies)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(bodies);
        }
    }
}
