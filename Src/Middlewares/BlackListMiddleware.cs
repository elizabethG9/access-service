using access_service.Src.Services.Interfaces;

namespace access_service.Src.Middlewares
{
    public class BlackListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBlackListService _blacklistService;

        public BlackListMiddleware(RequestDelegate next, IBlackListService blacklistService)
        {
            _next = next;
            _blacklistService = blacklistService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Replace("Bearer ", string.Empty);

                if (_blacklistService.IsBlacklisted(token))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Token is blacklisted");
                    return;
                }
            }

            await _next(context);
        }
    }
}