using Microsoft.Extensions.Options;
using Serilog;
using StackOverflowTagsApi.BLL.Helpers;
using StackOverflowTagsApi.BLL.Services;
using StackOverflowTagsApi.BLL.Services.Interfaces;
using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    
});
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<SQLiteConnection>(provider =>
{
    var options = provider.GetService<IOptions<AppSettings>>();
    return new SQLiteConnection(options?.Value.ConnectionString);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITagsService , TagsService>();
HttpClientHelper.InitializeClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run($"https://localhost:{app.Services.GetRequiredService<IOptions<AppSettings>>().Value.PortForAppHost}");
public partial class Program { }
