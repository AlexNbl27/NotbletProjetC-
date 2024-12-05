using Newtonsoft.Json;
using Notblet.Constants;
using Notblet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Notblet.Views
{
    /// <summary>
    /// Logique d'interaction pour GestionPage.xaml
    /// </summary>
    public partial class Stock : Page
    {
        private List<CategoryModel> Categories;

        public Stock()
        {
            InitializeComponent();
            LoadCategoriesAsync();
            LoadProductsAsync();
        }

        private async void LoadCategoriesAsync()
        {
            try
            {
                string response = await ApiService.Instance.GetDataAsync(endpoint: ApiConstants.Categories, token: SecureTokenStorage.Instance.token);
                Categories = JsonConvert.DeserializeObject<List<CategoryModel>>(response) ?? [];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des catégories : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                string response = await ApiService.Instance.GetDataAsync(endpoint: ApiConstants.Products, token: SecureTokenStorage.Instance.token);
                List<ProductModel> products = JsonConvert.DeserializeObject<List<ProductModel>>(response) ?? [];
                if (products.Count > 0)
                {
                    ProductsDataGrid.ItemsSource = products;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des produits : {ex.Message}");
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (Categories == null || Categories.Count == 0)
            {
                MessageBox.Show("Veuillez ajouter des catégories avant d'ajouter un produit.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Crée et affiche la boîte de dialogue pour ajouter un produit
            var addProductDialog = new ProductDialog(categories: Categories)
            {
                Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre modale
            };

            if (addProductDialog.ShowDialog() == true)
            {
                AddProductToDB(addProductDialog.Product);
            }
        }

        private void ProductsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductModel selectedProduct)
            {
                var editProductDialog = new ProductDialog(product: selectedProduct, categories: Categories)
                {
                    Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre
                };

                if (editProductDialog.ShowDialog() == true)
                {
                    UpdateProductInDB(editProductDialog.Product);
                }
            }
        }

        private async void AddProductToDB(ProductModel product)
        {
            try
            {
                await ApiService.Instance.PostDataAsync(
                    endpoint: ApiConstants.Products,
                    token: SecureTokenStorage.Instance.token,
                    jsonData: JsonConvert.SerializeObject(product)
                );

                // Actualiser la liste des produits après l'ajout
                await LoadProductsAsync();
                MessageBox.Show("Produit ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout du produit : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateProductInDB(ProductModel product)
        {
            try
            {
                await ApiService.Instance.PutDataAsync(
                                       endpoint: $"{ApiConstants.Products}/{product.id}",
                                                          token: SecureTokenStorage.Instance.token,
                                                                             jsonData: JsonConvert.SerializeObject(product)
                                                                                            );

                // Actualiser la liste des produits après la modification
                await LoadProductsAsync();
                MessageBox.Show("Produit modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification du produit : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteSelectedProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un ou plusieurs produits à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Voulez-vous vraiment supprimer les produits sélectionnés ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var product in ProductsDataGrid.SelectedItems.Cast<ProductModel>().ToList())
                {
                    DeleteProductInDB(product);
                }
            }      
        }

        private async void DeleteProductInDB(ProductModel product)
        {
            try
            {
                await ApiService.Instance.DeleteDataAsync(
                                       endpoint: ApiConstants.Products, id:product.id,
                                                          token: SecureTokenStorage.Instance.token
                                                                         );

                // Actualiser la liste des produits après la suppression
                await LoadProductsAsync();
                MessageBox.Show("Produit supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression du produit : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
