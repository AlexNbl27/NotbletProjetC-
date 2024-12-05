using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Notblet.Models;
using NotbletApi;

namespace NotbletApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserModelController : ControllerBase
    {
        private readonly dbaContext _context;
        private readonly string _jwtSecretKey = "P4vJ2UQkqHbVq7zZpEx7c9wPYdlM0uPz+OelwP5AlZY=";

        public UserModelController(dbaContext context)
        {
            _context = context;
        }

        // GET: api/UserModel/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUserModel(int id)
        {
            var userModel = await _context.users.FindAsync(id);

            if (userModel == null)
            {
                return NotFound();
            }

            return userModel;
        }

        // PUT: api/UserModel/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserModel(int id, UserModel userModel)
        {
            // Vérification si l'utilisateur authentifié correspond à l'utilisateur dans l'URL
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
            {
                return Unauthorized("Vous n'êtes pas autorisé à modifier cet utilisateur.");
            }

            if (id != userModel.id)
            {
                return BadRequest();
            }

            _context.Entry(userModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/UserModel/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserModel(int id)
        {
            // Vérification si l'utilisateur authentifié correspond à l'utilisateur dans l'URL
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != id.ToString())
            {
                return Unauthorized("Vous n'êtes pas autorisé à supprimer cet utilisateur.");
            }

            var userModel = await _context.users.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }

            _context.users.Remove(userModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool UserModelExists(int id)
        {
            return _context.users.Any(e => e.id == id);
        }

        // Inscription d'un nouvel utilisateur
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            // Vérifier si le nom d'utilisateur existe déjà
            if (_context.users.Any(u => u.username == user.username))
            {
                return BadRequest("Nom d'utilisateur déjà pris.");
            }

            // Validation des données d'entrée
            if (string.IsNullOrEmpty(user.username) || string.IsNullOrEmpty(user.password))
            {
                return BadRequest("Nom d'utilisateur et mot de passe sont obligatoires.");
            }

            // Hacher le mot de passe avant de l'enregistrer
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);

            // Enregistrer l'utilisateur dans la base de données
            _context.users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Utilisateur enregistré avec succès.");
        }

        // Connexion de l'utilisateur - Renvoie un token JWT après une connexion réussie
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserModel user)
        {
            // Trouver l'utilisateur par nom d'utilisateur
            var existingUser = await _context.users
                .FirstOrDefaultAsync(u => u.username == user.username);

            if (existingUser == null)
            {
                return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
            }

            // Vérifier le mot de passe avec le hachage stocké
            bool passwordMatch = BCrypt.Net.BCrypt.Verify(user.password, existingUser.password);

            if (!passwordMatch)
            {
                return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
            }

            // Créer un token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecretKey); // Clé secrète pour signer le JWT
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, existingUser.username),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, existingUser.id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Le token expire après 1 heure
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
}
