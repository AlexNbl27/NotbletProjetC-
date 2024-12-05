using System;
using System.Collections.Generic;

namespace Notblet.Models
{
    public class ClientModel
    {
        public int id { get; set; }  // Attribut en minuscule
        public required string name { get; set; }  // Attribut en minuscule
        public required string address { get; set; }  // Attribut en minuscule
        public required string siret { get; set; }  // Attribut en minuscule
    }
}
