using FastCaptcha.API.Services;
using FastCaptcha.Models.Captcha;
using Microsoft.AspNetCore.Mvc;

namespace FastCaptcha.API.Endpoints.CaptchaEndpoints;

public static class CaptchaEndpointV1
{
    public static void AddCaptchaV1DiServices(this IServiceCollection services)
    {
        services.AddScoped<ICaptchaServices, CaptchaServices>();
    }
    
    public static void MapV1(this RouteGroupBuilder app)
    {
        app
            .MapGet("generate-captcha", GenerateCaptcha);

        app
            .MapPost("verify-captcha", VerifyCaptcha);
    }


    private static IResult VerifyCaptcha([FromServices] ICaptchaServices services, [FromBody] VerifyCaptchaDto dto)
    {
        var response = services.VerifyCaptcha(dto);
        if (!response.Success)
        {
            return Results.BadRequest(response);
        }
        return Results.Ok(response);
    }

    private static async Task<IResult> GenerateCaptcha(ICaptchaServices services)
    {
        var apiKey = "";
        var captchaResponse = await services.GenerateCaptcha(apiKey);
        return Results.Ok(captchaResponse);
    }
}