using Microsoft.AspNetCore.Diagnostics;
using StargateAPI.Business.Data;
using StargateAPI.Business.Data.Repositories;

namespace StargateAPI.Controllers.Exceptions;

public class ExceptionHandler : IExceptionHandler
{
	private readonly IServiceProvider _serviceProvider;
	public ExceptionHandler(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		await LogErrorAsync(exception.Message);

		var response = new BaseResponse
		{
			Message = exception.Message,
			Success = false,
			ResponseCode = GetResponseCode(exception)
		};

		await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

		return true;
	}

	private int GetResponseCode(Exception exception) => exception switch
	{
		BadHttpRequestException => StatusCodes.Status400BadRequest,
		_ => StatusCodes.Status500InternalServerError
	};

	private async Task LogErrorAsync(string message)
	{
		using var scope = _serviceProvider.CreateScope();

		var logRepository = scope.ServiceProvider.GetRequiredService<IRepository<Log>>();

		await logRepository.AddAsync(new Log
		{
			Type = "Error",
			Details = message,
			CreatedDateTime = DateTime.Now
		});
	}
}
