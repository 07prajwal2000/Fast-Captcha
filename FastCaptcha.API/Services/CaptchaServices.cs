using FastCaptcha.Models;
using FastCaptcha.Models.Captcha;

namespace FastCaptcha.API.Services;

public class CaptchaServices : ICaptchaServices
{

    public string GenerateRandomText()
    {
        var charLength = 10;
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

    public string GenerateHash(string plainText)
    {
        
        return "";
    }
    
    public async Task<Response<GenerateCaptchaResponse>> GenerateCaptcha(string apiKey = null)
    {
        var captcha = new GenerateCaptchaResponse("", "", DateTime.Now);
        var response = new Response<GenerateCaptchaResponse>(captcha, "", true);

        return response;
    }
}

public interface ICaptchaServices
{
    Task<Response<GenerateCaptchaResponse>> GenerateCaptcha(string apiKey = null);
    string GenerateRandomText();
}