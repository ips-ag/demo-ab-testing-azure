using API.Services;
using Core.Entities.Identity;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Identity;
using Infrastructure.Services;
using IPSAG.AbTesting.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("local.settings.json", optional: true);
builder.AddAbTesting<TargetingContextService>(builder.Configuration.GetConnectionString("AppConfig")!,
                                              builder.Configuration.GetConnectionString("AppInsights")!);
//
// Add services to the container.
builder.Services.AddControllers()
    .AddAbTestingControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<EcommerceContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<ApplicationIdentityDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection"));
});
//
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();
builder.Services.AddScoped<ITokenGenerationService, TokenGenerationService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Replace with the actual Angular app URL.
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});
// Configure authentication and authorization middleware
var tokenSettings = builder.Configuration.GetSection("TokenSettings").Get<TokenSettings>()!;
var fileProvider = "appsettings.json";
var fileInfo = builder.Environment.ContentRootFileProvider.GetFileInfo(fileProvider);
Console.WriteLine("Using appsettings.json file at: " + fileInfo.PhysicalPath);
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = tokenSettings.Issuer,
        ValidAudience = tokenSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key))
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAbTesting();
//
app.MapControllers();
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == (int)HttpStatusCode.NotFound
        && !Path.HasExtension(context.Request.Path.Value)
        && context.Request.Path.Value != null
        && !context.Request.Path.Value.StartsWith("/api"))
    {
        context.Request.Path = "/index.html";
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        await next();
    }
});
app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = ["index.html"] });
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
});
//
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<EcommerceContext>();
// Create an instance of ILogger specifically for EcommerceContextSeed
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<EcommerceContextSeed>();

try
{
    await context.Database.MigrateAsync();
    // Create an instance of EcommerceContextSeed
    var ecommerceContextSeed = new EcommerceContextSeed(logger);
    await ecommerceContextSeed.SeedDataAsync(context);
    // Seed data for ApplicationIdentityDbContext
    await ApplicationIdentityDbContextSeed.SeedAsync(services);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occured during migration");
}
app.Run();