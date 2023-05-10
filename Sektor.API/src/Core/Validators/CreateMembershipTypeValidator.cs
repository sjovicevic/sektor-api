using FluentValidation;
using Sektor.API.src.Dtos;

namespace Sektor.API.src.Core.Validators;

public class CreateMembershipTypeValidator : AbstractValidator<MembershipTypeCreationDto>
{
    public CreateMembershipTypeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.");

        RuleFor(x => x.RegularPrice)
            .NotEmpty()
            .WithMessage("Regular price cannot be empty.");

        RuleFor(x => x.StudentPrice)
            .NotEmpty()
            .WithMessage("Student price cannot be empty.");
    }
}