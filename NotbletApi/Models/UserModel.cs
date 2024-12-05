using System;
using System.Collections.Generic;

namespace Notblet.Models
{
    public class UserModel
    {
        public int id { get; set; }  // Clé primaire en minuscule
        public required string username { get; set; }  // Nom d'utilisateur en minuscule
        public required string password { get; set; }  // Mot de passe
    }
}
