using JailLensApi.Data;
using JailLensApi.Infrastructure.IService;
using JailLensApi.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


Serilog.Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration, sectionName: "Serilog")
    .CreateLogger();

builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddDbContext<JailLensDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("JailLensDB"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IInmateService, InmateService>();
builder.Services.AddTransient<IFaceService, FaceService>();
builder.Services.AddTransient<IAlertsService, AlertsService>();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthorization();
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
