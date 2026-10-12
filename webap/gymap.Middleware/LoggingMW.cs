using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace gymap.Middleware
{
    public class LoggingMW
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMW> _logger;

        public LoggingMW(RequestDelegate next, ILogger<LoggingMW> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? string.Empty;

            // Debug ispis da odmah vidiš u konzoli da middleware radi
            Console.WriteLine($"[DEBUG MW] Primljen zahtjev: {method} {path}");

            // Preskačemo GET, OPTIONS i Swagger
            if (HttpMethods.IsGet(method) || 
                HttpMethods.IsOptions(method) || 
                path.Contains("swagger", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var timestamp = DateTime.UtcNow;

            var userIdentifier = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                 ?? context.User?.Identity?.Name 
                                 ?? "Anonimni korisnik";

            // Pusti zahtjev dalje do kontrolera/baze
            await _next(context);

            // Provjeravamo status kod (uključuje i 204 No Content za uspješan DELETE/PUT)
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                var logMessage = $"[AUDIT LOG] Vrijeme: {timestamp:yyyy-MM-dd HH:mm:ss} UTC | Korisnik: {userIdentifier} | Akcija: {method} {path} | Status: {context.Response.StatusCode}{Environment.NewLine}";

                _logger.LogInformation(logMessage);
                Console.WriteLine($"[DEBUG MW LOG] Uspješno snimljen log: {logMessage.Trim()}");

                try
                {
                    // Definišemo putanju direktno u korijenu aplikacije (tamo gdje je Program.cs / csproj)
                    var projectDirectory = Directory.GetCurrentDirectory();
                    var logFilePath = Path.Combine(projectDirectory, "audit_log.txt");

                    // Zapiši u datoteku (stvorit će je automatski ako ne postoji)
                    await File.AppendAllTextAsync(logFilePath, logMessage);
                    Console.WriteLine($"[DEBUG MW FILE] Datoteka uspješno ažurirana na putanji: {logFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG MW ERROR] Greška pri pisanju u datoteku: {ex.Message}");
                    _logger.LogError(ex, "Greška prilikom zapisivanja loga u datoteku.");
                }
            }
            else
            {
                Console.WriteLine($"[DEBUG MW SKIP] Status kod {context.Response.StatusCode} nije u rasponu 200-299.");
            }
        }
    }
}