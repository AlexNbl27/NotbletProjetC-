using System;
using System.Collections.Generic;

namespace Notblet.Models
{
    public class ProductModel
    {
        public int id { get; set; }  // Clé primaire
        public required string name { get; set; }
        public required decimal price { get; set; }
        public required DateTime expiration_date { get; set; }

        // Propriété de navigation
        public required CategoryModel category { get; set; }  // Propriété pour naviguer vers Category
        public required int category_id { get; set; }  // Clé étrangère pour Category
        public required string location { get; set; }
    }
}
