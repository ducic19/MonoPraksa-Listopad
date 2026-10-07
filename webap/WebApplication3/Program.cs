using WebApplication3.Common;
using WebApplication3.Model;
using WebApplication3.Repository;
using WebApplication3.Repository.Common;
using WebApplication3.Service;
using WebApplication3.Service.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- DEPENDENCY INJECTION REGISTRACIJA ---
builder.Services.AddSingleton<ISongRepository, SongRepository>();
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