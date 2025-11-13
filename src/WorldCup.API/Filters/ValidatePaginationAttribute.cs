using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WorldCup.Application.DTOs;

namespace WorldCup.API.Filters;

/// <summary>
/// Validates pagination parameters automatically.
/// </summary>
public class ValidatePaginationAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Get page parameter
        if (context.ActionArguments.TryGetValue("page", out var pageObj) && pageObj is int page)
        {
            if (page < 1)
            {
                context.Result = new BadRequestObjectResult(new ErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "VALIDATION_ERROR",
                        Message = "Page number must be >= 1.",
                        StatusCode = 400,
                        Details = new List<ErrorFieldDetail>
                        {
                            new() { Field = "page", Issue = $"Invalid value {page}" }
                        }
                    }
                });
                return;
            }
        }

        // Get pageSize parameter
        if (context.ActionArguments.TryGetValue("pageSize", out var pageSizeObj) && pageSizeObj is int pageSize)
        {
            if (pageSize < 1 || pageSize > 100)
            {
                context.Result = new BadRequestObjectResult(new ErrorResponse
                {
                    Error = new ErrorDetail
                    {
                        Code = "VALIDATION_ERROR",
                        Message = "Page size must be between 1 and 100.",
                        StatusCode = 400,
                        Details = new List<ErrorFieldDetail>
                        {
                            new() { Field = "pageSize", Issue = $"Invalid value {pageSize}" }
                        }
                    }
                });
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}
