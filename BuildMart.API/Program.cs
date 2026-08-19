using System.Text;
using BuildMart.API.Middleware;
using BuildMart.Application;
using BuildMart.Infrastructure;
using BuildMart.Infrastructure.Data;
using BuildMart.Infrastructure.Data.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Return our own uniform ApiResponse envelope for automatic [ApiController]
// model-binding failures too, so every 400 response has the same shape.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .SelectMany(kvp => kvp.Value?.Errors.Select(e => $"{kvp.Key}: {e.ErrorMessage}") ?? Array.Empty<string>())
            .ToList();

        return new BadRequestObjectResult(
            BuildMart.Application.DTOs.Common.ApiResponse<object>.FailureResponse("Validation failed.", errors));
    };
});

// ---------- CORS ----------

var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("BuildMartFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ---------- JWT Authentication ----------

var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwtSecret = jwtSection["SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Keep JWT claim types exactly as issued ("sub", not the ASP.NET-remapped ClaimTypes.NameIdentifier).
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();

// ---------- Swagger ----------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BuildMart API",
        Version = "v1",
        Description = "REST API for the BuildMart construction tools & materials e-commerce store."
    });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Paste ONLY the JWT token (no 'Bearer ' prefix — Swagger adds it automatically).",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    options.AddSecurityDefinition("Bearer", jwtScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtScheme, Array.Empty<string>() } });
});

var app = builder.Build();

// ---------- Apply migrations + seed data automatically on startup ----------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await IdentitySeeder.SeedAsync(app.Services);
}

// ---------- Middleware pipeline ----------

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger is enabled in every environment for now (including Production) to make
// initial deployment testing easier. Once the app is stable, wrap this back in
// `if (app.Environment.IsDevelopment())` to hide it from the public in production.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BuildMart API v1");
    options.RoutePrefix = "swagger";
});

// Note: HTTPS redirection is handled by the IIS/reverse-proxy edge on
// MonsterASP.NET (Force HTTPS Redirect is enabled there). Since IIS
// terminates TLS and forwards requests to Kestrel as plain HTTP,
// also calling UseHttpsRedirection() here would make the app think
// every request is HTTP and try to redirect again, causing a conflict.
// app.UseHttpsRedirection();

app.UseCors("BuildMartFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
