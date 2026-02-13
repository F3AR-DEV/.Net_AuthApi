using AuthApi.Core.Persistence;
using AuthApi.Shared.Docs;
using AuthApi.Shared.DependencyInjection;
using AuthApi.Shared.Security;
using AuthApi.Shared.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Servicios (IOC)
builder.Services.AddControllers();
builder.Services.AddCorsPolicy();
builder.Services.AddApiVersioningConfig();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Seguridad y Servicios de Aplicación
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseSwaggerDocumentation(app.Environment);

app.UseCors("AllowAll");
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
