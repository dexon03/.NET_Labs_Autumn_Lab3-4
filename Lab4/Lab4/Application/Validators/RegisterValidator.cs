using FluentValidation;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;

namespace Lab4.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator(IRepository repository)
    {
        RuleFor(r => r.FirstName)
            .NotEmpty();
        RuleFor(r => r.LastName)
            .NotEmpty();
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is not valid")
            .Custom((email, context) =>
            {
                if (repository.Any<User>( x => x.Email == email))
                {
                    context.AddFailure("Email should be unique");
                }
            });
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            .Matches(@"[\!\?\*\._]+").WithMessage("Your password must contain at least one non alphanumeric character.");
    }
}