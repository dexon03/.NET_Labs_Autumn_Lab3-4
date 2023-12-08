using Lab4.Application.Services;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;
using MockQueryable.Moq;
using Moq;

namespace Lab4UnitTests;

public class UserManagerTests
{
    private readonly Mock<IRepository> _mockRepository;

    public UserManagerTests()
    {
        _mockRepository = new Mock<IRepository>();
    }

    [Fact]
    public async Task TestGetUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User(),
            new User()
        };
        var mockUsers = users.AsQueryable().BuildMock();

        _mockRepository.Setup(repo => repo.GetAll<User>())
            .Returns(mockUsers);

        var userManager = new UserManager(_mockRepository.Object);

        // Act
        var result = await userManager.GetUsers();

        // Assert
        _mockRepository.Verify(repo => repo.GetAll<User>(), Times.Once());
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task TestFindByEmailAsync()
    {
        // Arrange
        var testEmail = "test@email.com";
        var user = new User { Email = testEmail };
        var users = new List<User> { user };
        var mockUsers = users.AsQueryable().BuildMock();

        _mockRepository.Setup(repo => repo.GetAll<User>())
            .Returns(mockUsers);

        var userManager = new UserManager(_mockRepository.Object);

        // Act
        var result = await userManager.FindByEmailAsync(testEmail);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(user.Email, result.Value.Email);
    }

    [Fact]
    public async Task TestFindByIdAsync()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var user = new User { Id = testId };

        _mockRepository.Setup(repo => repo.GetByIdAsync<User>(It.IsAny<Guid>()))
            .ReturnsAsync(user);

        var userManager = new UserManager(_mockRepository.Object);

        // Act
        var result = await userManager.FindByIdAsync(testId);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task TestDeleteUser()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var user = new User { Id = testId };

        _mockRepository.Setup(repo => repo.GetByIdAsync<User>(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _mockRepository.Setup(repo => repo.Delete(It.IsAny<User>()));
        _mockRepository.Setup(repo => repo.SaveChangesAsync(CancellationToken.None));

        var userManager = new UserManager(_mockRepository.Object);

        // Act
        var result = await userManager.DeleteUser(testId);

        // Assert
        Assert.False(result.IsError);
        Assert.True(result.Value);
        _mockRepository.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Once());
        _mockRepository.Verify(repo => repo.SaveChangesAsync(CancellationToken.None), Times.Once());
    }
}