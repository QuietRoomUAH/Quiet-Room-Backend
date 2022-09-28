using FastEndpoints;
using Google.Cloud.Diagnostics.AspNetCore3;
using QuietRoom.Server.Startup;

var builder = WebApplication.CreateBuilder(args);
builder.Services.UseSupabase();
builder.Services.AddFastEndpoints();
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Services.AddGoogleDiagnosticsForAspNetCore();
}
// Sets up open CORS, needed for StopLight
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseAuthorization();
// Sets up the endpoints to have no authentication
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
