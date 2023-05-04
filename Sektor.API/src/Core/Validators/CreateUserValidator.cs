using FluentValidation;
using Sektor.API.src.Dtos;

namespace Sektor.API.src.Core.Validators;

public class CreateUserValidator : AbstractValidator<UserCreationDto>
{
    public CreateUserValidator() 
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name cannot be empty.");
        RuleFor(x => x.FirstName)
            .MaximumLength(20)
            .WithMessage("First name maximum length is 20.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name cannot be empty.");
        RuleFor(x => x.LastName)
            .MaximumLength(20)
            .WithMessage("Last name maximum length is 20.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Must be email address.");
    }
}