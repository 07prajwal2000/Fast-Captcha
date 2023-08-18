namespace FastCaptcha.Models.Captcha;

public record GenerateCaptchaResponse(string Hash, string ImageAsBase64, long Expiry);