using FastCaptcha.API.Endpoints;
using FastCaptcha.Hashing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHashDIServices();
builder.Services.AddEndpointDIServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapMinimalApiEndpoints();

app.Run();