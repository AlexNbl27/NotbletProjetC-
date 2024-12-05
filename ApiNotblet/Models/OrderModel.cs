using System;
using System.Collections.Generic;

namespace Notblet.Models
{
    public class OrderModel
    {
        public int id { get; set; }  // Clé primaire en minuscule
        public required int quantity { get; set; }  // Quantité en minuscule
        public required DateTime order_date { get; set; }  // Date de commande en minuscule
        public required string status { get; set; }  // Statut de la commande en minuscule

        public required ClientModel client { get; set; }  // Propriété de navigation vers Client
        public required int client_id { get; set; }  // Clé étrangère vers Client en minuscule
        public required ProductModel product { get; set; }  // Propriété de navigation vers Product
        public required int product_id { get; set; }  // Clé étrangère vers Product en minuscule
    }
}
