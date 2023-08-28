using FastCaptcha.Hashing;
using FastCaptcha.ImageProcessing;
using FastCaptcha.Models;
using FastCaptcha.Models.Captcha;

namespace FastCaptcha.API.Services;

public class CaptchaServices : ICaptchaServices
{
    private readonly ShaHashService _hashService;
    private readonly ImageProcessor _imageProcessor;

    public CaptchaServices(ShaHashService hashService, ImageProcessor imageProcessor)
    {
        this._hashService = hashService;
        _imageProcessor = imageProcessor;
    }

    private string GenerateRandomText()
    {
        var charLength = 8;
        var rand = Random.Shared;
        Span<char> spanOfChars = stackalloc char[charLength];
        for (int i = 0; i < charLength; i++)
        {
            var type = rand.Next(1, 4); // 1=number;2=small_char;3=capital;
            char genChar = '0';
            if (type == 1)
            {
                var number = rand.Next(48, 57);
                genChar = Convert.ToChar(number);
            }
            else if (type == 2)
            {
                var smallAlpha = rand.Next(97, 123);
                genChar = Convert.ToChar(smallAlpha);
            }
            else
            {
                var bigAlpha = rand.Next(65, 91);
                genChar = Convert.ToChar(bigAlpha);
            }
            spanOfChars[i] = genChar;
        }

        var captchaText = new string(spanOfChars);
        
        return captchaText;
    }

    private string GenerateHash(string plainText)
    {
        var hash = _hashService.HashData(plainText);
        return hash;
    }
    
    public async Task<Response<GenerateCaptchaResponse>> GenerateCaptcha(string apiKey = null)
    {
        var captchaText = GenerateRandomText();
        var hash = GenerateHash(captchaText);
        var imageAsBase64 = await Task.Run(() => _imageProcessor.GenerateImageFromText(captchaText));
        
        var captcha = new GenerateCaptchaResponse(hash, imageAsBase64, DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds());
        var response = new Response<GenerateCaptchaResponse>(captcha, "Success", true);
        
        return response;
    }

    public Response<VerifyCaptchaResponse> VerifyCaptcha(VerifyCaptchaDto dto)
    {
        try
        {
            var valid = _hashService.Validate(dto.UserInput, dto.Hash);
            var response = new Response<VerifyCaptchaResponse>(
                new VerifyCaptchaResponse(valid ? "The input matches with the captcha." : "The input might be incorrect or the captcha has already expired.", valid), valid ? "Success" : "Failed", valid);
            return response;
        }
        catch (Exception)
        {
            return new Response<VerifyCaptchaResponse>(new VerifyCaptchaResponse("Invalid captcha", false), "Failed",
                false);
        }
    }
}

public interface ICaptchaServices
{
    Task<Response<GenerateCaptchaResponse>> GenerateCaptcha(string apiKey = null);
    Response<VerifyCaptchaResponse> VerifyCaptcha(VerifyCaptchaDto dto);
}