using FastCaptcha.API.Endpoints;
using FastCaptcha.Hashing;
using FastCaptcha.ImageProcessing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHashDIServices();
builder.Services.AddEndpointDIServices();
builder.Services.AddImageProcessorServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapMinimalApiEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.Run();