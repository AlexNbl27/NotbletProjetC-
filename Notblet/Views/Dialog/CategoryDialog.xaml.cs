using Notblet.Models;
using System.Windows;

namespace Notblet.Views
{
    public partial class CategoryDialog : Window
    {
        public CategoryModel Category { get; set; }

        public CategoryDialog(CategoryModel? category = null)
        {
            InitializeComponent();
            category ??= new CategoryModel { name = "" };
            Category = category;
            CategoryNameTextBox.Text = Category.name;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Le nom de la catégorie ne peut pas être vide", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Récupérer le nom de la catégorie à partir de la TextBox et l'affecter à l'objet Category
            Category.name = CategoryNameTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
