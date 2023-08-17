using FastCaptcha.API.Endpoints.CaptchaEndpoints;

namespace FastCaptcha.API.Endpoints;

public static class EndpointsMapper
{
    public static void MapMinimalApiEndpoints(this WebApplication app)
    {
        app.AddCaptchaEndpoints();
    }

    public static void AddEndpointDIServices(this IServiceCollection services)
    {
        services.AddCaptchaEndpointsDIServices();
    }
}