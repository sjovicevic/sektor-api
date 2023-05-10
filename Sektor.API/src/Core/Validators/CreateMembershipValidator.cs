using FluentValidation;
using Sektor.API.src.Dtos;

namespace Sektor.API.src.Core.Validators;

public class CreateMembershipValidator : AbstractValidator<MembershipCreationDto>
{
    public CreateMembershipValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date cannot be empty.");
        
        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date cannot be empty.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date cannot be less than start date.");
    }
}