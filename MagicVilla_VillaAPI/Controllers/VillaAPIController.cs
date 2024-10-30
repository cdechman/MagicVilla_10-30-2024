using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    // API Controller is built in support for data requirements
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;

        public VillaAPIController(ILogger<VillaAPIController> logger)
        {
            _logger = logger;
        }

        private readonly ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext db) 
        {
            _db = db;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //IEnumerable is the return type
        // adding Action Result so we can return a Ok message
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.LogInformation("Getting all villas");
            return Ok(_db.Villas.ToList());
        }
        
        // adding Action Result so we can return a Ok message
        [HttpGet("{id:int}", Name = "GetVilla")]
        // cleaing up responce in Swagger
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Getting Villa with id" + id);
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        // adding a record
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {

            // valadations
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details=villaDTO.Details,
                Id=villaDTO.Id,
                ImageUrl=villaDTO.ImageUrl,
                Name=villaDTO.Name,
                Occupancy=villaDTO.Occupancy,
                Rate=villaDTO.Rate,
                Sqft=villaDTO.Sqft
            };
            _db.Villas.Add(model);
            _db.SaveChanges();
            
            //VillaStore.villaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        // deleting a record
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletVilla(int id) 
        {
            if (id == 0) 
            { 
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return NoContent();
        }

        // update a record
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            // valadate the ids match
            if (villaDTO == null || id!= villaDTO.Id)
            {
                return BadRequest();
            }
            // code to update all of the record
            //var villa = VillaStore.villaList.FirstOrDefault(u =>u.Id == id);
            //villa.Name= villaDTO.Name;
            //villa.Sqft= villaDTO.Sqft;
            //villa.Occupancy= villaDTO.Occupancy;

            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };
            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
        }

        // update a partial record
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO) 
        { 
            if (patchDTO == null || id ==0)
            { 
                return BadRequest(); 
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);

            VillaDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft
            };


            if (villa == null) 
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = new Villa()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };

            _db.Villas.Update(model);
            _db.SaveChanges();

            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }

    public class Controllerbase
    {
    }
}
