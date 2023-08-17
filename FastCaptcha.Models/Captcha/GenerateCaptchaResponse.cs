namespace FastCaptcha.Models.Captcha;

public record GenerateCaptchaResponse(string Hash, string ImageAsBase64, DateTime Expiry);