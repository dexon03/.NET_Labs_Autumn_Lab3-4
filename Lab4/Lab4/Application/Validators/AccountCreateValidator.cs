using FluentValidation;
using Lab4.Domain.Dtos;
using Lab4.Domain.Entities;
using Lab4.Infrastructure.Database.Repository;

namespace Lab4.Application.Validators;

public class AccountCreateValidator : AbstractValidator<AccountDto>
{
    public AccountCreateValidator(IRepository repository)
    {
        RuleFor(a => a.AccountName)
            .NotEmpty()
            .MaximumLength(50)
            .Custom((accName, context) =>
            {
                if (repository.Any<Account>(x => x.AccountName == accName))
                {
                    context.AddFailure("Account name should be unique");
                }
            });
        RuleFor(a => a.UserId)
            .NotEmpty();
    }
}