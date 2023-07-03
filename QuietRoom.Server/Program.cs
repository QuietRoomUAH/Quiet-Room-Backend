using FastEndpoints;
using Google.Cloud.Diagnostics.AspNetCore3;
using QuietRoom.Server.Middleware;
using QuietRoom.Server.Startup;

var builder = WebApplication.CreateBuilder(args);
// Sets up open CORS, needed for StopLight
builder.Services.AddCors(options => 
    options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.UseSupabase();
builder.Services.AddFastEndpoints();
builder.Services.AddResponseCaching();
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.ClearProviders();
    builder.Services.AddGoogleDiagnosticsForAspNetCore();
}

var app = builder.Build();
app.UseCors();
app.UseRemoveContentTypeMiddleware();
app.UseResponseCaching();
app.UseAuthorization();
app.UseFastEndpoints(config =>
{
    config.Endpoints.Configurator = ep =>
    {
        ep.AllowAnonymous();
    };
});
app.UseDefaultExceptionHandler();
if (app.Environment.IsDevelopment()) app.Run();
// Set the app to run based on google cloud port
var url = $"http://0.0.0.0:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}";
app.Run(url);
