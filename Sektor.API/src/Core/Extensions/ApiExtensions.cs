using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Sektor.API.src.Core.Errors;

namespace Sektor.API.src.Core.Extensions;

public static class ApiExtensions
{
    public static IActionResult AsClientErrors(this ValidationResult result)
    {
        var errorMessages = result.Errors.Select(x => new ClientError
        {
            ErrorMessage = x.ErrorMessage,
            PropertyName = x.PropertyName
        });
        return new UnprocessableEntityObjectResult(new
        {
            Errors = errorMessages
        });
    }
}