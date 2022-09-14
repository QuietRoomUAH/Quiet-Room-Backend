using FastEndpoints;
using Google.Cloud.Diagnostics.AspNetCore3;
using QuietRoom.Server.Startup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.UseTestRoomRetriever();
builder.Services.AddFastEndpoints();
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Services.AddGoogleDiagnosticsForAspNetCore();
}

var app = builder.Build();
app.UseAuthorization();
app.UseFastEndpoints(config =>
{
    config.Endpoints.Configurator = ep =>
    {
        ep.AllowAnonymous();
    };
});
app.UseCors();
if (app.Environment.IsDevelopment()) app.Run();
// Set the app to run based on google cloud port
var url = $"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}";
app.Run(url);
