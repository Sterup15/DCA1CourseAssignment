using Microsoft.AspNetCore.Mvc;

namespace VEA.Presentation.WebApi.Endpoints.Common;

public abstract class ApiEndpoint
{
    [ApiController]
    public abstract class WithRequest<TRequest> : ControllerBase
    {
        public abstract Task<IActionResult> HandleAsync(TRequest request, CancellationToken ct = default);

        [ApiController]
        public abstract class AndResponse<TResponse> : ControllerBase
        {
            public abstract Task<ActionResult<TResponse>> HandleAsync(TRequest request, CancellationToken ct = default);
        }
    }

    [ApiController]
    public abstract class WithResponse<TResponse> : ControllerBase
    {
        public abstract Task<ActionResult<TResponse>> HandleAsync(CancellationToken ct = default);
    }
}
