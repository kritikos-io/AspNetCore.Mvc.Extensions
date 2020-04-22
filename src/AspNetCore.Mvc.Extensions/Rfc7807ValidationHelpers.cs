namespace Kritikos.AspNetCore.Mvc.Extensions
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Infrastructure;
	using Microsoft.Extensions.DependencyInjection;

	public static class Rfc7807ValidationHelpers
	{
		public static IMvcBuilder AddRfc7807ProblemReporting(this IMvcBuilder builder)
			=> builder.ConfigureApiBehaviorOptions(action =>
			{
				action.InvalidModelStateResponseFactory = ctx =>
				{
					var problemFactory = ctx.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
					var problemDetails = problemFactory.CreateValidationProblemDetails(ctx.HttpContext, ctx.ModelState);

					problemDetails.Detail = "See the errors field for more details.";
					problemDetails.Instance = ctx.HttpContext.Request.Path;

					var actionExecutingContext =
							 ctx as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

					var properParameterCount = actionExecutingContext?.ActionArguments.Count == ctx.ActionDescriptor.Parameters.Count;

					if (ctx.ModelState.ErrorCount > 0 && properParameterCount)
					{
						problemDetails.Title = "One or more validation errors occurred.";
						problemDetails.Status = StatusCodes.Status422UnprocessableEntity;

						return new UnprocessableEntityObjectResult(problemDetails)
						{
							ContentTypes = { "application/problem+json" },
						};
					}

					problemDetails.Title = "One or more errors occurred at input.";
					problemDetails.Status = StatusCodes.Status400BadRequest;

					return new BadRequestObjectResult(problemDetails)
					{
						ContentTypes = { "application/problem+json" },
					};
				};
			});
	}
}
