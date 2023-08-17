using FastCaptcha.API.Services;
using FastCaptcha.Hashing;
using Microsoft.AspNetCore.Mvc;

namespace FastCaptcha.API.Endpoints.CaptchaEndpoints;

public static class CaptchaEndpointV1
{
    public static void MapV1(this RouteGroupBuilder app)
    {
        app
            .MapGet("generate-captcha", GenerateCaptcha);
        
        app
            .MapGet("gen", GenerateRandom);

        app
            .MapGet("hash", GenerateHash);

        app
            .MapGet("validate", Validate);
    }
    
    public static void AddCaptchaV1DIServices(this IServiceCollection services)
    {
        services.AddScoped<ICaptchaServices, CaptchaServices>();
    }

    private static IResult GenerateRandom(ICaptchaServices services)
    {
        var captchaText = services.GenerateRandomText();
        return Results.Ok(captchaText);
    }

    private static IResult GenerateHash([FromServices]ShaHashService hashService, [FromQuery] string rawText)
    {
        return Results.Ok(hashService.HashData(rawText));
    }
    
    private static IResult Validate([FromServices] ShaHashService hashService, [FromQuery] string rawText, [FromQuery] string cipher)
    {
        var valid = hashService.Validate(rawText, cipher);
        return Results.Ok(new {rawText, cipher, valid});
    }

    private static IResult GenerateCaptcha(ICaptchaServices services)
    {
        return Results.Ok();
    }
}