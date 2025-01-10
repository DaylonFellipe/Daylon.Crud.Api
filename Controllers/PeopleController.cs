using Daylon.Crud.Api.DataAccess;
using Daylon.Crud.Api.Model;
using Daylon.Crud.Api.Request;
using Daylon.Crud.Api.UseCases.Person;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Daylon.Crud.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PeopleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPerson()
        {
            var person = await _context.People
                .AsNoTracking()
                .ToListAsync();

            if (person.Count == 0)
                return NotFound("No person found");

            return Ok(person);
        }

        [HttpGet("specific")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpecificPerson(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name is required");

            var person = await _context.People
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name!.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (person == null)
                return NotFound("Person not found");

            return Ok(
                person.Name + " "
                + person.LastName + ", "
                + person.Age);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostPerson([FromBody] RequestRegisterPersonJson request)
        {
            //Validar
            var validator = new PersonValidator();

            var result = validator.Validate(request);

            if (!result.IsValid)
                return BadRequest("Invalid data");

            //Mapear
            var person = new PersonModel
            {
                Name = request.Name,
                LastName = request.LastName,
                Age = request.Age
            };

            //Salvar
            await _context.People.AddAsync(person);

            await _context.SaveChangesAsync();

            return Created(string.Empty, person.Name);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id!.Equals(id));

            if (person == null)
                return NotFound("Person not found");

            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] RequestRegisterPersonJson request)
        {
            //Busca a entidade
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id!.Equals(id));

            if (person == null)
                return NotFound("Person not found");

            //Atualiza
            var validator = new PersonValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
                return BadRequest("Invalid data");

            person.Name = request.Name;
            person.LastName = request.LastName;
            person.Age = request.Age;

            //Salva
            await _context.SaveChangesAsync();

            return Ok($"Person {person.Name} was successfully updated.");
        }

        [HttpPatch("name")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePersonName(int id, string newName)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id!.Equals(id));

            if (person == null)
                return NotFound("Person not found");

            if (newName == string.Empty || newName.Length > 70)
                return BadRequest("Invalid name");

            person.Name = newName;

            await _context.SaveChangesAsync();

            return Ok($"The new name is {person.Name}");
        }

        [HttpPatch("last name")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePersonLastName(int id, string newLastName)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id!.Equals(id));

            if (person == null)
                return NotFound("Person not found");

            if (newLastName == string.Empty || newLastName.Length > 100)
                return BadRequest("Invalid name");

            person.LastName = newLastName;

            await _context.SaveChangesAsync();

            return Ok($"The new last name is {person.LastName}");
        }

        [HttpPatch("age")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePersonAge(int id, int newAge)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id!.Equals(id));

            var age = person!.Age;

            if (person == null)
                return NotFound("Person not found");

            if (newAge <= age)
                return BadRequest("Age must be greater than the previous one");

            person.Age = newAge;

            await _context.SaveChangesAsync();

            return Ok($"The new age is {person.Age}");
        }
    }
}

//   ░░░░░░░░░░░░░░░░▄▄█▀▀██▄▄░░░░░░░
//   ░░░░░░░░░░░░░▄█▀▀░░░░░░░▀█░░░░░░
//   ░░░░░░░░░░░▄▀░░░░░░░░░░░░░█░░░░░
//   ░░░░░░░░░▄█░░░░░░░░░░░░░░░█░░░░░
//   ░░░░░░░██▀░░░░░░░▄▄▄░░▄░█▄█▄░░░░
//   ░░░░░▄▀░░░░░░░░░░████░█▄██░▀▄░░░
//   ░░░░█▀░░░░░░░░▄▄██▀░░█████░██░░░
//   ░░░█▀░░░░░░░░░▀█░▀█▀█▀▀▄██▄█▀░░░
//   ░░░██░░░░░░░░░░█░░█░█░░▀▀▄█▀░░░░
//   ░░░░█░░░░░█░░░▀█░░░░▄░░░░░▄█░░░░
//   ░░░░▀█░░░░███▄░█░░░░░░▄▄▄▄█▀█▄░░
//   ░░░░░▀██░░█▄▀▀██░░░░░░░░▄▄█░░▀▄░
//   ░░░░░░▀▀█▄░▀▄▄░▄░░░░░░░███▀░░▄██
//   ░░░░░░░░░▀▀▀███▀█▄░░░░░█▀░▀░░░▀█
//   ░░░░░░░░░░░░▄▀░░░▀█▄░░░░░▄▄░░▄█▀
//   ░░░▄▄▄▀▀▀▀▀█▀░░░░░█▄▀▄▄▄▄▄▄█▀▀░░
//   ░▄█░░░▄██▀░░░░░░░░░█▄░░░░░░░░░░░
//   █▀▀░▄█░░░░░░░░░░░░░░▀▀█▄░░░░░░░░
//   █░░░█░░░░░░░░░░░░░░░░░░█▄░░░░░░░   D A Y L O N