using FluentValidation;
using Lab4.Domain.Dtos;

namespace Lab4.Application.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(l => l.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(l => l.Password)
            .NotEmpty();
    }
}