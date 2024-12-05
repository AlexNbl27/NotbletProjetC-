using Newtonsoft.Json;
using Notblet.Constants;
using Notblet.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Notblet.Views
{
    /// <summary>
    /// Logique d'interaction pour Categories.xaml
    /// </summary>
    public partial class Categories : Page
    {
        public Categories()
        {
            InitializeComponent();
            LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                string response = await ApiService.Instance.GetDataAsync(endpoint: ApiConstants.Categories, token: SecureTokenStorage.Instance.token);
                List<CategoryModel> categories = JsonConvert.DeserializeObject<List<CategoryModel>>(response) ?? [];
                if(categories.Count > 0)
                {
                    CategoriesDataGrid.ItemsSource = categories;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des catégories : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Crée et affiche la boîte de dialogue pour ajouter une catégorie
            var addCategoryDialog = new CategoryDialog
            {
                Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre modale
            };

            if (addCategoryDialog.ShowDialog() == true)
            {
                AddCategoryToDB(addCategoryDialog.Category);
            }
        }

        private void CategoriesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is CategoryModel selectedCategory)
            {
                var editCategoryDialog = new CategoryDialog(category: selectedCategory)
                {
                    Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre
                };

                if (editCategoryDialog.ShowDialog() == true)
                {
                    UpdateCategoryInDB(editCategoryDialog.Category);
                }
            }
        }

        private async void AddCategoryToDB(CategoryModel category)
        {
            try
            {
                await ApiService.Instance.PostDataAsync(endpoint: ApiConstants.Categories, token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(category));

                // Actualiser la liste des catégories après l'ajout
                await LoadCategoriesAsync();
                MessageBox.Show("Catégorie ajoutée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateCategoryInDB(CategoryModel category)
        {
            try
            {
                await ApiService.Instance.PutDataAsync(
                                       endpoint: $"{ApiConstants.Categories}/{category.id}",
                                                          token: SecureTokenStorage.Instance.token,
                                                                             jsonData: JsonConvert.SerializeObject(category)
                                                                                            );

                // Actualiser la liste des produits après la modification
                await LoadCategoriesAsync();
                MessageBox.Show("Produit modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification du produit : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSelectedCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is CategoryModel selectedCategory)
            {
                if (MessageBox.Show("Voulez-vous vraiment supprimer cette catégorie ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DeleteCategoryInDB(selectedCategory);
                }
            }
        }

        private async void DeleteCategoryInDB(CategoryModel category)
        {
            try
            {
                await ApiService.Instance.DeleteDataAsync(endpoint: ApiConstants.Categories, id:category.id, token: SecureTokenStorage.Instance.token);

                // Actualiser la liste des catégories après suppression
                await LoadCategoriesAsync();
                MessageBox.Show("Catégorie supprimée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression de la catégorie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
