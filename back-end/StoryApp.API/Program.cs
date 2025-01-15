using Serilog;
using StoryApp.API;

var builder = WebApplication.CreateBuilder(args);

// Add Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureLogging(builder.Host);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
