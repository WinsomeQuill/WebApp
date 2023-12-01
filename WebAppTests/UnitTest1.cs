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
    public async Task Test()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: "PersonListDatabase")
            .Options;

        var context = new ApplicationContext(options);

        await context.Persons.AddRangeAsync(
            new Person
            {
                Id = 1, Name = "artem", DisplayName = "Artem",
                Skills = new List<Skill> { new Skill { Name = "C#", Level = 4 } }
            },
            new Person
            {
                Id = 2, Name = "artem", DisplayName = "Artem",
                Skills = new List<Skill> { new Skill { Name = "Rust", Level = 7 } }
            }
        );

        await context.SaveChangesAsync();
        
        var mock = new Mock<IRepository>();
        var mockLogger = new Mock<ILogger<PersonsController>>();
        mock.Setup(repo=>repo.GetAll()).Returns(GetTestUsers());
        var controller = new PersonsController(mockLogger.Object, context);

        var result = await controller.Persons();
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Person>>(okResult.Value);
        Assert.Equal(GetTestUsers().Count, model.Count());
    }
    
    private List<Person> GetTestUsers()
    {
        List<Person> persons = new List<Person>
        {
            new Person { Id = 1, Name = "artem", DisplayName = "Artem", Skills = new List<Skill> { new Skill { Name = "C#", Level = 4 } } },
            new Person { Id = 2, Name = "artem", DisplayName = "Artem", Skills = new List<Skill> { new Skill { Name = "Rust", Level = 7 } } }
        };
        return persons;
    }
}