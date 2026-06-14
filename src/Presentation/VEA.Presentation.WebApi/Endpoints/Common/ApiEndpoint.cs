using Microsoft.AspNetCore.Mvc;

namespace VEA.Presentation.WebApi.Endpoints.Common;

// Endpoint with both request and response
[ApiController]
public abstract class ApiEndpoint<TRequest, TResponse> : ControllerBase
{
    public abstract Task<ActionResult<TResponse>> HandleAsync(TRequest request, CancellationToken ct = default);
}

// Endpoint with request, no typed response (commands returning status only)
[ApiController]
public abstract class ApiEndpoint<TRequest> : ControllerBase
{
    public abstract Task<IActionResult> HandleAsync(TRequest request, CancellationToken ct = default);
}

// Endpoint with no request body, with response
[ApiController]
public abstract class ApiEndpointNoInput<TResponse> : ControllerBase
{
    public abstract Task<ActionResult<TResponse>> HandleAsync(CancellationToken ct = default);
}