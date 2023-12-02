using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebApp.Controllers;
using WebApp.Models;

namespace WebAppTests;

public interface IRepository
{
    IEnumerable<Person> GetAll();
}

public class UnitTest1
{
    [Fact]
    public async Task TestGetPersons()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: "PersonListDatabase")
            .Options;

        var context = new ApplicationContext(options);
        await context.Database.EnsureDeletedAsync();

        await context.Persons.AddRangeAsync(GetTestPersons());

        await context.SaveChangesAsync();
        
        var mock = new Mock<IRepository>();
        var mockLogger = new Mock<ILogger<PersonsController>>();
        mock.Setup(repo=>repo.GetAll()).Returns(GetTestPersons());
        var controller = new PersonsController(mockLogger.Object, context);

        var result = await controller.Persons();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Person>>(okResult.Value);
        Assert.Equal(GetTestPersons().Count, model.Count());
    }
    
    private List<Person> GetTestPersons()
    {
        List<Person> persons = new List<Person>
        {
            new Person { Id = 1, Name = "artem", DisplayName = "Artem", Skills = new List<Skill> { new Skill { Name = "C#", Level = 4 } } },
            new Person { Id = 2, Name = "artem", DisplayName = "Artem", Skills = new List<Skill> { new Skill { Name = "Rust", Level = 7 } } }
        };
        return persons;
    }
    
    [Fact]
    public async Task TestCreatePerson()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: "PersonListDatabase")
            .Options;

        var context = new ApplicationContext(options);
        await context.Database.EnsureDeletedAsync();

        var mock = new Mock<IRepository>();
        var mockLogger = new Mock<ILogger<PersonsController>>();
        mock.Setup(repo=>repo.GetAll()).Returns(GetTestPersons());
        var controller = new PersonsController(mockLogger.Object, context);

        PersonDto personDto = new()
        {
            Name = "alex",
            DisplayName = "SuperAlex",
            Skills = new List<SkillDto>()
            {
                new()
                {
                    Name = "C++",
                    Level = 9,
                }
            },
        };
        
        Person resultPerson = new()
        {
            Id = 1,
            Name = "alex",
            DisplayName = "SuperAlex",
            Skills = new List<Skill>()
            {
                new()
                {
                    Id = 1,
                    Name = "C++",
                    Level = 9,
                }
            },
        };
        
        var result = await controller.CreatePerson(personDto);
        _ = Assert.IsType<OkResult>(result);
        
        result = await controller.Persons();
        var okResultObject = Assert.IsType<OkObjectResult>(result);
        
        var model = Assert.IsAssignableFrom<List<Person>>(okResultObject.Value);
        Assert.Single(model);
        Assert.Equal(resultPerson.DisplayName, model[0].DisplayName);
        Assert.Equal(resultPerson.Name, model[0].Name);
        Assert.Single(model[0].Skills);
        Assert.Equal(resultPerson.Skills[0].Name, model[0].Skills[0].Name);
        Assert.Equal(resultPerson.Skills[0].Level, model[0].Skills[0].Level);
    }
    
    
}