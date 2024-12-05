using Newtonsoft.Json;
using Notblet.Constants;
using Notblet.Models;
using NLog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Notblet.Views
{
    /// <summary>
    /// Logique d'interaction pour GestionPage.xaml
    /// </summary>
    public partial class Stock : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); // Initialisation du logger
        private List<CategoryModel> Categories;

        public Stock()
        {
            InitializeComponent();
            Logger.Info("Page Stock initialisée.");
            LoadCategoriesAsync();
            LoadProductsAsync();
        }

        private async void LoadCategoriesAsync()
        {
            try
            {
                Logger.Info("Chargement des catégories...");
                string response = await ApiService.Instance.GetDataAsync(endpoint: ApiConstants.Categories, token: SecureTokenStorage.Instance.token);
                Categories = JsonConvert.DeserializeObject<List<CategoryModel>>(response) ?? new List<CategoryModel>();
                Logger.Info("Catégories chargées avec succès.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors du chargement des catégories.");
                MessageBox.Show($"Erreur lors du chargement des catégories : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                Logger.Info("Chargement des produits...");
                string response = await ApiService.Instance.GetDataAsync(endpoint: ApiConstants.Products, token: SecureTokenStorage.Instance.token);
                List<ProductModel> products = JsonConvert.DeserializeObject<List<ProductModel>>(response) ?? new List<ProductModel>();
                if (products.Count > 0)
                {
                    ProductsDataGrid.ItemsSource = products;
                    Logger.Info($"{products.Count} produits chargés.");
                }
                else
                {
                    Logger.Warn("Aucun produit trouvé.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors du chargement des produits.");
                MessageBox.Show($"Erreur lors du chargement des produits : {ex.Message}");
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Tentative d'ajout d'un produit...");
            if (Categories == null || Categories.Count == 0)
            {
                Logger.Warn("Aucune catégorie disponible pour l'ajout d'un produit.");
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
                Logger.Info("Produit ajouté via le dialogue.");
                AddProductToDB(addProductDialog.Product);
            }
            else
            {
                Logger.Warn("L'ajout de produit a été annulé.");
            }
        }

        private void ProductsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductModel selectedProduct)
            {
                Logger.Info($"Édition du produit {selectedProduct.name}...");
                var editProductDialog = new ProductDialog(product: selectedProduct, categories: Categories)
                {
                    Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre
                };

                if (editProductDialog.ShowDialog() == true)
                {
                    Logger.Info("Produit modifié via le dialogue.");
                    UpdateProductInDB(editProductDialog.Product);
                }
            }
        }

        private async void AddProductToDB(ProductModel product)
        {
            try
            {
                Logger.Info($"Ajout du produit {product.name} à la base de données...");
                await ApiService.Instance.PostDataAsync(
                    endpoint: ApiConstants.Products,
                    token: SecureTokenStorage.Instance.token,
                    jsonData: JsonConvert.SerializeObject(product)
                );

                // Actualiser la liste des produits après l'ajout
                await LoadProductsAsync();
                Logger.Info("Produit ajouté avec succès.");
                MessageBox.Show("Produit ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors de l'ajout du produit.");
                MessageBox.Show($"Erreur lors de l'ajout du produit : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateProductInDB(ProductModel product)
        {
            try
            {
                Logger.Info($"Modification du produit {product.name} dans la base de données...");
                await ApiService.Instance.PutDataAsync(
                    endpoint: $"{ApiConstants.Products}/{product.id}",
                    token: SecureTokenStorage.Instance.token,
                    jsonData: JsonConvert.SerializeObject(product)
                );

                // Actualiser la liste des produits après la modification
                await LoadProductsAsync();
                Logger.Info("Produit modifié avec succès.");
                MessageBox.Show("Produit modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors de la modification du produit.");
                MessageBox.Show($"Erreur lors de la modification du produit : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteSelectedProductsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItems.Count == 0)
            {
                Logger.Warn("Aucun produit sélectionné pour suppression.");
                MessageBox.Show("Veuillez sélectionner un ou plusieurs produits à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Voulez-vous vraiment supprimer les produits sélectionnés ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var product in ProductsDataGrid.SelectedItems.Cast<ProductModel>().ToList())
                {
                    Logger.Info($"Suppression du produit {product.name}...");
                    DeleteProductInDB(product);
                }
            }
        }

        private async void DeleteProductInDB(ProductModel product)
        {
            try
            {
                Logger.Info($"Suppression du produit {product.name} de la base de données...");
                await ApiService.Instance.DeleteDataAsync(
                    endpoint: ApiConstants.Products, id: product.id,
                    token: SecureTokenStorage.Instance.token
                );

                // Actualiser la liste des produits après la suppression
                await LoadProductsAsync();
                Logger.Info("Produit supprimé avec succès.");
                MessageBox.Show("Produit supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors de la suppression du produit.");
                MessageBox.Show($"Erreur lors de la suppression du produit : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
