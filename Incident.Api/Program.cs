using Incident.Api.Configurations;
using Incident.Api.DI;
using Incident.Application.Interfaces.ServiceCatalogClient;
using Incident.Infrastructure.ExternalServices;
using Incident.Infrastructure.Identity;
using Incident.Infrastructure.Persistence.Mongo;
using Incident.Infrastructure.Persistence.Relational;
using Incident.Infrastructure.Persistence.Relational.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// HttpContext
builder.Services.AddHttpContextAccessor();

// DBContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services
    .AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// CORS
var corsHostClients = builder.Configuration
    .GetSection("CORSDomainClients")
    .Get<string[]>() ?? throw new InvalidOperationException("Add a client domain for CORS to CORSDomainClients env(array)");

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "origen1",
        app =>
        {
            app
            .WithOrigins(corsHostClients)
            .AllowAnyHeader()
            .AllowAnyMethod();
        }
    );
});

// Http Client
var catalogUrl = builder.Configuration["ServiceCatalog:BaseUrl"];
builder.Services.AddHttpClient<IServiceCatalogClient, ServiceCatalogClient>(client =>
{
    client.BaseAddress = new Uri(catalogUrl!);
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configurations
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings")); // MongoDB
builder.Services.ConfigureSwagger(); // Swagger
builder.Services.ConfigureJwt(builder.Configuration); // JWT

// Dependecy Injection
builder.Services.InjectionContainer();

// Use App
var app = builder.Build();

app.UseApiConfiguration();
app.UseCors("origen1");

await app.ValidAutomaticMigration();

// Seed de roles y usuarios por defecto
await DbInitializerRepository.SeedAsync(app.Services);

await app.RunAsync();
