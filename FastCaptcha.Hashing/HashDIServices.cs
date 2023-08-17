using Microsoft.Extensions.DependencyInjection;

namespace FastCaptcha.Hashing;

public static class HashDIServices
{
    public static void AddHashDIServices(this IServiceCollection services)
    {
        services.AddScoped<ShaHashService>();
    }
}