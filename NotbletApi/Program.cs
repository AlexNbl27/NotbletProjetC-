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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("P4vJ2UQkqHbVq7zZpEx7c9wPYdlM0uPz+OelwP5AlZY=")), // Cl� secr�te
            ClockSkew = TimeSpan.Zero // Pour une validation stricte de l'expiration
        };
    });

// Ajout des services n�cessaires
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<dbaContext>(options => options.UseSqlite("Data Source=database.db"));

var app = builder.Build();

// Assurez-vous que la base de donn�es est cr��e si elle n'existe pas
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<dbaContext>();
    dbContext.Database.EnsureCreated(); // Cr�e la base de donn�es si elle n'existe pas
}

// Configuration de Swagger uniquement en mode d�veloppement
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = ""; // Cela rend Swagger disponible � la racine (e.g., localhost:8080)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();

