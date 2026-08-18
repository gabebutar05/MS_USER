using Microsoft.EntityFrameworkCore;
using MS_USER.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register DbContext — SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("inTechgraConnectionString")));


// Register MediatR — scan semua Handler di assembly ini
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Konfigurasi OpenAPI (native .NET 10)
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "MS_USER API";
        document.Info.Version = "v1";
        document.Info.Description = "Microservice untuk manajemen user dan autentikasi";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Swagger / API UI menggunakan Scalar (pengganti Swagger UI untuk .NET 10)
app.MapOpenApi();                           // endpoint: /openapi/v1.json
app.MapScalarApiReference(options =>
{
    options.Title = "MS_USER API";
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});                                         // UI tersedia di: /scalar/v1

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
