using SynonymsSearchTool.Application.Interfaces;
using SynonymsSearchTool.Application.Services;
using SynonymsSearchTool.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS service and define a CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()   // Allow any origin (you can restrict it later)
               .AllowAnyMethod()   // Allow any HTTP method (GET, POST, etc.)
               .AllowAnyHeader();  // Allow any header
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ISynonymService, SynonymService>();
builder.Services.AddSingleton<SynonymDomainService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Apply CORS policy globally
app.UseCors("AllowAll");  // This line enables CORS with the "AllowAll" policy

app.UseAuthorization();

app.MapControllers();

app.Run();