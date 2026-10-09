using gymap.Repository; // Donosi MemberRepository
using gymap.Repository.Common; // Donosi IMemberRepository
using Microsoft.EntityFrameworkCore;
using gymap.Service; // Donosi MemberService
using gymap.Service.Common; // Donosi IMemberService

var builder = WebApplication.CreateBuilder(args);

// 1. JSON opcije protiv kružnih petlji
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 2. Konekcija na bazu preko AppDbContext-a
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Registracija servisa i repozitorija
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberService, MemberService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();