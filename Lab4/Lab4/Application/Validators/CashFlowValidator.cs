using FluentValidation;
using Lab4.Domain.Dtos;

namespace Lab4.Application.Validators;

public class CashFlowValidator : AbstractValidator<CashFlowDto>
{
    public CashFlowValidator()
    {
        RuleFor(c => c.Source)
            .NotEmpty();
    }
}