using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorldCup.Application.DTOs;

namespace WorldCup.API.Filters;

/// <summary>
/// Validates gender parameter automatically.
/// </summary>
public class ValidateGenderAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Get gender parameter
        if (context.ActionArguments.TryGetValue("gender", out var genderObj) 
            && genderObj is string gender 
            && !string.IsNullOrEmpty(gender))
        {
            if (!gender.Equals("male", StringComparison.OrdinalIgnoreCase) && 
                !gender.Equals("female", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new BadRequestObjectResult(new ErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "VALIDATION_ERROR",
                        Message = "Invalid gender value. Must be 'male' or 'female'.",
                        StatusCode = 400,
                        Details = new List<ErrorFieldDetail>
                        {
                            new() { Field = "gender", Issue = $"Invalid value '{gender}'" }
                        }
                    }
                });
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}
