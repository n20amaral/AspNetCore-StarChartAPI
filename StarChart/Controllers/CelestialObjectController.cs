﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if(celestialObject == null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(String name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name.Equals(name)).ToList();

            if(celestialObjects.Count == 0)
            {
                return NotFound();
            }


            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }


            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach(var celestialObject in celestialObjects) {
                celestialObject.Satellites = celestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var objectToUpdate = _context.CelestialObjects.Find(id);

            if(objectToUpdate == null)
            {
                return NotFound();
            }

            objectToUpdate.Name = celestialObject.Name;
            objectToUpdate.OrbitalPeriod = celestialObject.OrbitalPeriod;
            objectToUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(objectToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var objectToUpdate = _context.CelestialObjects.Find(id);

            if(objectToUpdate == null)
            {
                return NotFound();
            }

            objectToUpdate.Name = name;

            _context.CelestialObjects.Update(objectToUpdate);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objectsToDelete = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();

            if(!objectsToDelete.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(objectsToDelete);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
