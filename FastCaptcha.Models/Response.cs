namespace FastCaptcha.Models;

public record Response<T>(T Data, string Message, bool Success);