namespace FastCaptcha.Models.Captcha;

public record VerifyCaptchaDto(string Hash, string UserInput);