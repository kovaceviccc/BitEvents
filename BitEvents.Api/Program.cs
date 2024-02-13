using System.Text;
using BitEvents.Api.Database;
using BitEvents.Api.Domain.Enums;
using BitEvents.Api.Endpoints.Internal;
using BitEvents.Api.Middleware;
using BitEvents.Api.Repositories;
using BitEvents.Api.Repositories.Interfaces;
using BitEvents.Api.Services;
using BitEvents.Api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Repositories
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ILocationRepository, LocationRepository>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<IEventRepository, EventRepository>();

// Services
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ICategoryService, CategoryService>();
builder.Services.AddSingleton<ILocationService, LocationService>();
builder.Services.AddSingleton<IEventService, EventService>();

// Database
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Validators
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Authentication
builder.Services.AddAuthentication()
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(builder.Configuration["Authorization:Secret"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(RolesEnum.Admin.ToString(), policy =>
    {
        policy.AuthenticationSchemes = new List<string> { JwtBearerDefaults.AuthenticationScheme };
        policy.RequireClaim("Roles", RolesEnum.Admin.ToString());
    });
    options.AddPolicy(RolesEnum.User.ToString(), policy =>
    {
        policy.AuthenticationSchemes = new List<string> { JwtBearerDefaults.AuthenticationScheme };
        policy.RequireAssertion(ctx => ctx.User.HasClaim("Roles", RolesEnum.Admin.ToString())
                                       || ctx.User.HasClaim("Roles", RolesEnum.User.ToString()));
    });
});

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services.GetService<IUserRepository>()!,
    app.Services.GetService<ILocationRepository>()!,
    app.Services.GetService<ICategoryRepository>()!);

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionMiddleware>();

app.UseEndpoints<Program>();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/");
    return Task.CompletedTask;
});

app.Run();
