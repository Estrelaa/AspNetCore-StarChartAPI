using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChangesAsync();

            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var body = _context.CelestialObjects.Find(id);
            if (body == null)
            {
                return NotFound();
            }

            ApplyUpdate(celestialObject, body);

            return NoContent();
        }

        private void ApplyUpdate(CelestialObject celestialObject, CelestialObject body)
        {
            body.Name = celestialObject.Name;
            body.OrbitalPeriod = celestialObject.OrbitalPeriod;
            body.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.SaveChangesAsync();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var body = _context.CelestialObjects.Find(id);
            if (body == null)
            {
                return NotFound();
            }

            body.Name = name;
            _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Id == id).ToList();
            if (!celestialObjects.Any())
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
