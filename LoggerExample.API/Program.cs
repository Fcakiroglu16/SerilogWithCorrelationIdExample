

using LoggerExample.API.Middlewares;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var logConfiguration = new LoggerConfiguration()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
           .Enrich.FromLogContext()
           .WriteTo.Console(outputTemplate:
           "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId}  {Message:lj}{NewLine}{Exception}")

           .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog(logConfiguration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
