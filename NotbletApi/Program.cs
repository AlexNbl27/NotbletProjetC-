using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NotbletApi;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration de l'authentification JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("P4vJ2UQkqHbVq7zZpEx7c9wPYdlM0uPz+OelwP5AlZY=")), // Clé secrète
            ClockSkew = TimeSpan.Zero // Pour une validation stricte de l'expiration
        };
    });

// Ajout des services nécessaires
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<dbaContext>(options => options.UseSqlite("Data Source=database.db"));

var app = builder.Build();

// Assurez-vous que la base de données est créée si elle n'existe pas
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<dbaContext>();
    dbContext.Database.EnsureCreated(); // Crée la base de données si elle n'existe pas
}

// Configuration de Swagger uniquement en mode développement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = ""; // Cela rend Swagger disponible à la racine (e.g., localhost:8080)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();

