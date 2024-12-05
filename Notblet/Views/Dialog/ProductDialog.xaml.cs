using Notblet.Models;
using System.Windows;

namespace Notblet.Views
{
    public partial class ProductDialog : Window
    {
        public ProductModel Product { get; set; }
        private List<CategoryModel> Categories;

        public ProductDialog(List<CategoryModel> categories, ProductModel? product = null)
        {
            InitializeComponent();
            DataContext = this;
            Categories = categories;
            product ??= new ProductModel
            {
                name = string.Empty,
                price = 0,
                expiration_date = DateTime.Now,
                category = categories.First(),
                category_id = categories.First().id,
                location = string.Empty
            };
            Product = product;

            // Remplir les champs avec les informations du produit
            ProductNameTextBox.Text = Product.name;
            ProductPriceTextBox.Text = Product.price.ToString("0.00");
            ProductExpirationDatePicker.SelectedDate = Product.expiration_date;
            ProductLocationTextBox.Text = Product.location;

            // Remplir la liste déroulante des catégories
            ProductCategoryComboBox.ItemsSource = Categories;
            ProductCategoryComboBox.DisplayMemberPath = "name";
            ProductCategoryComboBox.SelectedValuePath = "id";
            ProductCategoryComboBox.SelectedValue = Product.category_id;
            ProductCategoryComboBox.SelectedItem = Product.category;
        }



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text) || string.IsNullOrWhiteSpace(ProductPriceTextBox.Text) || ProductCategoryComboBox.SelectedItem == null || ProductExpirationDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Tous les champs obligatoires n'ont pas été remplis", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Récupérer les informations du produit
            Product.name = ProductNameTextBox.Text;
            Product.price = decimal.TryParse(ProductPriceTextBox.Text, out decimal price) ? price : 0;
            Product.expiration_date = ProductExpirationDatePicker.SelectedDate ?? DateTime.MinValue;
            Product.category = (CategoryModel)ProductCategoryComboBox.SelectedItem;
            Product.category_id = Product.category.id;
            Product.location = ProductLocationTextBox.Text;

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
