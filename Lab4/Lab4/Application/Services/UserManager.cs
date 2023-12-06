using ErrorOr;
using Lab4.Application.Utilities;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Application.Services;

public class UserManager
{
    private readonly IRepository _repository;

    public UserManager(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<User>> GetUsers()
    {
        return await (from u in _repository.GetAll<User>()
            select new User
            {
                Id = u.Id,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                PasswordSalt = u.PasswordSalt,
                FirstName = u.FirstName,
                LastName = u.LastName,
            }).ToListAsync(); 
    }
    
    public async Task<ErrorOr<User>> FindByEmailAsync(string email)
    {
        var user =  await (from u in _repository.GetAll<User>()
            where u.Email == email
            select new User
            {
                Id = u.Id,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                PasswordSalt = u.PasswordSalt,
                FirstName = u.FirstName,
                LastName = u.LastName,
            }).AsNoTracking().FirstOrDefaultAsync();
        if (user == null)
        {
            return Error.NotFound(description: "Wrong email");
        }
        return user;
    } 
    
    public async Task<ErrorOr<User>> FindByIdAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync<User>(id);
        if (user == null)
        {
            return Error.NotFound(description: "User not found by id");
        }

        return user;
    }
    
    public Task<bool> CheckPasswordAsync(User user, string password)
    {
        var hashedPassword = PasswordUtility.GetHashedPassword(password, user.PasswordSalt);
        return Task.FromResult(hashedPassword == user.PasswordHash);
    }
    
    public async Task<ErrorOr<bool>> DeleteUser(Guid id)
    {
        var user = await FindByIdAsync(id);
        if (user.IsError)
        {
            return user.Errors;
        }
        _repository.Delete(user.Value);
        await _repository.SaveChangesAsync();
        return true;
    }
}