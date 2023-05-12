using Sektor.API.src.Services;
using Sektor.API.src.Context;
using Sektor.API.src.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sektor.API.src.Contracts; 
using Moq;


namespace Sektor.API.Tests;

public class UserRepositoryTest
{
    [Fact]
    public void GetAllUsers_ShouldReturnAllUsers()
    {
        var data = new List<User>
        {
            new User { FirstName = "Stefan", LastName = "Jovicevic", Student = true, Email = "jovicevicst@icloud.com"},
            new User { FirstName = "Marko", LastName = "Nikolic", Student = true, Email = "marko@icloud.com"},
            new User { FirstName = "Milan", LastName = "Jovanovic", Student = true, Email = "milan@icloud.com"},
            new User { FirstName = "Andjela", LastName = "Mitrovic", Student = true, Email = "andjela@icloud.com"},
            new User { FirstName = "Milos", LastName = "Andjelic", Student = true, Email = "milos@icloud.com"}
        }.AsQueryable();

        var mockSet = new Mock<DbSet<User>>();

        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        DbContextOptions<SektorContext> DbContextOptions = new DbContextOptions<SektorContext>();
        var mockContext = new Mock<SektorContext>(DbContextOptions);
        mockContext.Setup(c => c.Users).Returns(mockSet.Object);

        IUserRepository repository = new UserRepository(mockContext.Object);
        var users = repository.GetAllUsers();

        Assert.Equal(5, users.Count());
        Assert.Equal("Stefan", users.ElementAt(0).FirstName);
        Assert.Equal("Jovicevic", users.ElementAt(0).LastName);
        Assert.Equal("Nikolic", users.ElementAt(1).LastName);
        Assert.Equal("Milan", users.ElementAt(2).FirstName);
        Assert.Equal("Andjela", users.ElementAt(3).FirstName);
        Assert.Equal("Milos", users.ElementAt(4).FirstName);
    }
}