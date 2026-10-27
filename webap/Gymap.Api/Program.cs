using System.Text;
using Gymap.Api.Middleware;
using Gymap.DAL;
using Gymap.DAL.Members;
using Gymap.DAL.Subscriptions;
using Gymap.DAL.Trainers;
using Gymap.Service.Auth;
using Gymap.Service.Members;
using Gymap.Service.Subscriptions;
using Gymap.Service.Trainers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// 1. Inicijalizacija builder-a
var builder = WebApplication.CreateBuilder(args);

// 2. Registracija DbContext-a (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Dodavanje kontrolera i Swaggera
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Registracija servisa i repozitorija (Dependency Injection)
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 5. Konfiguracija JWT Autentifikacije
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtSettings:Key"] 
            ?? throw new InvalidOperationException("CRITICAL ERROR: 'JwtSettings:Key' nedostaje u konfiguraciji/User Keys!");

        var issuer = builder.Configuration["JwtSettings:Issuer"] 
            ?? throw new InvalidOperationException("CRITICAL ERROR: 'JwtSettings:Issuer' nije definiran u appsettings.json!");

        var audience = builder.Configuration["JwtSettings:Audience"] 
            ?? throw new InvalidOperationException("CRITICAL ERROR: 'JwtSettings:Audience' nije definiran u appsettings.json!");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

var app = builder.Build();

// 6. Gymap.Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();  

// --- OVDJE JE DODAN MIDDLEWARE ---
app.UseMiddleware<LoggingMW>(); 
// ---------------------------------

app.MapControllers();

var scope = app.Services.CreateScope();
try
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (builder.Configuration.GetValue("Database:AutoMigrate", true))
    {
        dbContext.Database.Migrate();
        Console.WriteLine("[INFO] Sustav je uspješno povezan s bazom podataka.");
    }
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(dbEx, "Greška pri upisu, molimo provjerite unesene podatke");
    throw;
}
catch (TimeoutException timeoutEx)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(timeoutEx, "Molimo pokušajte ponovo kasnije!.");
    throw;
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Dogodila se neočekivana pogreška");
    throw;
}
finally
{
    scope.Dispose();
}
app.Run();
