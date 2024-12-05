using Newtonsoft.Json;
using Notblet.Constants;
using Notblet.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NLog;

namespace Notblet.Views
{
    /// <summary>
    /// Logique d'interaction pour Categories.xaml
    /// </summary>
    public partial class Categories : Page
    {
        // Logger pour la classe Categories
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Categories()
        {
            InitializeComponent();
            LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                Logger.Info("Chargement des catégories...");
                string response = await ApiService.Instance.GetDataAsync(endpoint: ApiConstants.Categories, token: SecureTokenStorage.Instance.token);
                List<CategoryModel> categories = JsonConvert.DeserializeObject<List<CategoryModel>>(response) ?? new List<CategoryModel>();

                if (categories.Count > 0)
                {
                    CategoriesDataGrid.ItemsSource = categories;
                    Logger.Info($"Chargé {categories.Count} catégories.");
                }
                else
                {
                    Logger.Warn("Aucune catégorie trouvée.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors du chargement des catégories.");
                MessageBox.Show($"Erreur lors du chargement des catégories : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Ouverture du dialogue pour ajouter une catégorie.");
            // Crée et affiche la boîte de dialogue pour ajouter une catégorie
            var addCategoryDialog = new CategoryDialog
            {
                Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre modale
            };

            if (addCategoryDialog.ShowDialog() == true)
            {
                Logger.Info("Ajout de la catégorie.");
                AddCategoryToDB(addCategoryDialog.Category);
            }
        }

        private void CategoriesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is CategoryModel selectedCategory)
            {
                Logger.Info($"Modification de la catégorie avec ID: {selectedCategory.id}");

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
                Logger.Info($"Ajout de la catégorie : {category.name}");
                await ApiService.Instance.PostDataAsync(endpoint: ApiConstants.Categories, token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(category));

                // Actualiser la liste des catégories après l'ajout
                await LoadCategoriesAsync();
                MessageBox.Show("Catégorie ajoutée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors de l'ajout de la catégorie.");
                MessageBox.Show($"Erreur lors de l'ajout de la catégorie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateCategoryInDB(CategoryModel category)
        {
            try
            {
                Logger.Info($"Mise à jour de la catégorie avec ID: {category.id}");
                await ApiService.Instance.PutDataAsync(
                    endpoint: $"{ApiConstants.Categories}/{category.id}",
                    token: SecureTokenStorage.Instance.token,
                    jsonData: JsonConvert.SerializeObject(category)
                );

                // Actualiser la liste des catégories après la modification
                await LoadCategoriesAsync();
                MessageBox.Show("Catégorie modifiée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors de la modification de la catégorie.");
                MessageBox.Show($"Erreur lors de la modification de la catégorie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSelectedCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is CategoryModel selectedCategory)
            {
                Logger.Info($"Demande de suppression de la catégorie avec ID: {selectedCategory.id}");

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
                Logger.Info($"Suppression de la catégorie avec ID: {category.id}");
                await ApiService.Instance.DeleteDataAsync(endpoint: ApiConstants.Categories, id: category.id, token: SecureTokenStorage.Instance.token);

                // Actualiser la liste des catégories après suppression
                await LoadCategoriesAsync();
                MessageBox.Show("Catégorie supprimée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors de la suppression de la catégorie.");
                MessageBox.Show($"Erreur lors de la suppression de la catégorie : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
