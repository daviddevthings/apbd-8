using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ITripsService, TripsService>();
builder.Services.AddScoped<IClientsService, ClientsService>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
var app = builder.Build();


app.MapControllers();
app.Run();