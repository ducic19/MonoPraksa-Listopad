using Microsoft.EntityFrameworkCore;
using WebApplication3.Repository; // Ovdje se nalazi tvoj AppDbContext
using WebApplication3.Common;
using WebApplication3.Model;
using WebApplication3.Repository.Common;
using WebApplication3.Service;
using WebApplication3.Service.Common;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 1. Registracija AppDbContext-a s Npgsql (PostgreSQL) konekcijom
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// 2. Register your repositories and services
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<ISongService, SongService>();

// --- DEPENDENCY INJECTION REGISTRACIJA ---
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddSingleton<IIdGenerator, IdGenerator>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IRequestCounter, RequestCounter>();
builder.Services.AddTransient<ILoggerNotifier, LoggerNotifier>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();