using System.Collections.Generic;
using System.Linq;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HotelManagementSystem.Api.Attributes
{
    public class ValidateModelAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(PrepareValidationError(context.ModelState));
            }
            base.OnActionExecuting(context);
        }

        private List<ErrorModel> PrepareValidationError(ModelStateDictionary modeState)
        {
            var errors = new List<ErrorModel>();
            foreach (var error in modeState)
            {
                errors.Add(new ErrorModel()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Title = error.Key,
                    ErrorMessage = string.Join("\n",error.Value.Errors.Select(x => x.ErrorMessage).ToList())
                });
            }

            return errors;
        }
    }
}
