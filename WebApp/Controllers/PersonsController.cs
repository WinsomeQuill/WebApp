using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly ILogger<PersonsController> _logger;
        private readonly ApplicationContext _db;
        
        public PersonsController(ILogger<PersonsController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        async public Task<IActionResult> Persons()
        {
            try
            {
                List<Person> persons = await _db.Persons
                    .Include(p => p.Skills)
                    .ToListAsync();
                return Ok(persons);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(500);
            }
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Person))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        async public Task<IActionResult> Person(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id cannot negative!");
                }
                
                _logger.LogDebug($"Get person by id: {id}", DateTime.UtcNow.ToLongTimeString());
                Person? person = await _db.Persons.Include(p => p.Skills).FirstOrDefaultAsync(x => x.Id == id);

                if (person == null)
                {
                    return NotFound("Person not found");
                }
                
                return Ok(person);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(500);
            }
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        async public Task<IActionResult> CreatePerson(PersonDto personDto)
        {
            try
            {
                List<Skill> skills = new List<Skill>();
                foreach (SkillDto skillDto in personDto.Skills)
                {
                    Skill skill = new()
                    {
                        Level = skillDto.Level,
                        Name = skillDto.Name
                    };
                    
                    skills.Add(skill);
                }

                Person person = new()
                {
                    Name = personDto.Name,
                    DisplayName = personDto.DisplayName,
                    Skills = skills
                };
                
                await _db.Persons.AddAsync(person);
                await _db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(500);
            }
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        async public Task<IActionResult> UpdatePerson(long id, PersonDto personDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id cannot negative!");
                }
                
                _logger.LogDebug($"Update person by id: {id}", DateTime.UtcNow.ToLongTimeString());
                Person? findPerson = await _db.Persons.Include(p => p.Skills).FirstOrDefaultAsync(x => x.Id == id);
                if (findPerson == null)
                {
                    return NotFound("Person not found!");
                }

                List<Skill> skills = new List<Skill>();

                // Перебираем скиллы
                // Это позволяет использовать существующие объекты
                foreach (SkillDto skillDto in personDto.Skills)
                {
                    // Ищем скилл в бд
                    Skill? findSkill = await _db.Skills.FirstOrDefaultAsync(x => x.Name == skillDto.Name);
                    // Если найден, то добавляем скилл из бд в список
                    if (findSkill != null)
                    {
                        findSkill.Level = skillDto.Level;
                        skills.Add(findSkill);
                        continue;
                    }

                    // Если не найден, то добавляем скилл в список и при SaveChanges данные скилл создасться в бд
                    Skill skill = new()
                    {
                        Name = skillDto.Name,
                        Level = skillDto.Level
                    };
                    skills.Add(skill);
                }

                findPerson.Skills = skills;
                findPerson.Name = personDto.Name;
                findPerson.DisplayName = personDto.DisplayName;

                await _db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(500);
            }
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        async public Task<IActionResult> DeletePerson(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Id cannot negative!");
                }
                
                Person? findPerson = await _db.Persons.FirstOrDefaultAsync(x => x.Id == id);
                if (findPerson == null)
                {
                    return NotFound("Person not found!");
                }

                _db.Persons.Remove(findPerson);
                await _db.SaveChangesAsync();
                    
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, DateTime.UtcNow.ToLongTimeString());
                return StatusCode(500);
            }
        }
    }
}