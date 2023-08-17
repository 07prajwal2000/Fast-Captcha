namespace FastCaptcha.API.Endpoints.CaptchaEndpoints;

public static class CaptchaEndpoints
{
    public static WebApplication AddCaptchaEndpoints(this WebApplication app)
    {
        app.MapGroup("api/v1/")
            .MapV1();
        
        return app;
    }

    public static void AddCaptchaEndpointsDIServices(this IServiceCollection services)
    {
        services.AddCaptchaV1DIServices();
    }
}