using System.Text;
using ImmobilienVerwalter.API.Middleware;
using ImmobilienVerwalter.API.Services;
using ImmobilienVerwalter.Core.Interfaces;
using ImmobilienVerwalter.Infrastructure;
using ImmobilienVerwalter.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// === Database ===
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// === Repositories & UnitOfWork ===
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// === Services ===
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// === Authentication ===
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT Secret ist nicht konfiguriert. Setze die Umgebungsvariable JWT_SECRET.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// === Controllers & Swagger ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// === Health Checks ===
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

// === CORS (konfigurierbar) ===
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// === Global Exception Handler ===
app.UseMiddleware<GlobalExceptionHandler>();

// === Middleware Pipeline ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// === Auto-Migration ===
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        db.Database.Migrate();
        logger.LogInformation("Datenbank-Migration erfolgreich durchgeführt.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Fehler bei der Datenbank-Migration. Versuche EnsureCreated als Fallback.");
        try
        {
            db.Database.EnsureCreated();
        }
        catch (Exception ex2)
        {
            logger.LogError(ex2, "EnsureCreated ebenfalls fehlgeschlagen. App startet ohne DB-Migration.");
        }
    }
}

app.Run();
