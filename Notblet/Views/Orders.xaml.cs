using Newtonsoft.Json;
using Notblet.Constants;
using Notblet.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Notblet.Views
{
    /// <summary>
    /// Logique d'interaction pour Clients.xaml
    /// </summary>
    public partial class Orders : Page
    {
        List<ClientModel> Clients;
        List<ProductModel> Products;

        public Orders()
        {
            InitializeComponent();
            LoadClientsAsync();
            LoadProductsAsync();
            LoadOrdersAsync();
        }

        private async void LoadClientsAsync()
        {
            try
            {
                string token = SecureTokenStorage.Instance.token;
                string response = await ApiService.Instance.GetDataAsync(ApiConstants.Clients, token: token);
                Clients = JsonConvert.DeserializeObject<List<ClientModel>>(response) ?? [];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des catégories : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadProductsAsync()
        {
            try
            {
                string token = SecureTokenStorage.Instance.token;
                string response = await ApiService.Instance.GetDataAsync(ApiConstants.Products, token: token);
                Products = JsonConvert.DeserializeObject<List<ProductModel>>(response) ?? [];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des produits : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                string token = SecureTokenStorage.Instance.token;
                string response = await ApiService.Instance.GetDataAsync(ApiConstants.Orders, token: token);
                List<OrderModel> orders = JsonConvert.DeserializeObject<List<OrderModel>>(response) ?? [];
                if (orders.Count > 0)
                {
                    OrdersDataGrid.ItemsSource = orders;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des produits : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clients == null || Clients.Count == 0)
            {
                MessageBox.Show("Veuillez ajouter des clients avant de passer une commande.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(Products == null || Products.Count == 0)
            {
                MessageBox.Show("Veuillez ajouter des produits avant de passer une commande.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var addOrderDialog = new OrderDialog(clients: Clients, products: Products)
            {
                Owner = Window.GetWindow(this)
            };

            if (addOrderDialog.ShowDialog() == true)
            {
                AddOrderToDB(addOrderDialog.Order);
            }
        }

        private void OrdersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Vérifier si un élément est sélectionné
            if (OrdersDataGrid.SelectedItem is OrderModel selectedOrder)
            {
                // Ouvrir la boîte de dialogue pour modifier la commande
                var editOrderDialog = new OrderDialog(order: selectedOrder, clients: Clients, products: Products)
                {
                    Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre
                };

                if (editOrderDialog.ShowDialog() == true)
                {
                    // Mettre à jour la commande dans la base de données après modification
                    UpdateOrderInDB(editOrderDialog.Order);
                }
            }
        }

        private async void DeleteSelectedOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier si des commandes sont sélectionnées dans le DataGrid
            var selectedOrders = OrdersDataGrid.SelectedItems.Cast<OrderModel>().ToList();

            if (selectedOrders.Count == 0)
            {
                MessageBox.Show("Aucune commande sélectionnée pour suppression.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Confirmer la suppression
            var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer {selectedOrders.Count} commandes ?",
                                         "Confirmer la suppression",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var order in selectedOrders)
                    {
                        await DeleteOrderFromDB(order);
                    }

                    // Actualiser la liste des commandes après suppression
                    await LoadOrdersAsync();
                    MessageBox.Show("Commandes supprimées avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression des commandes : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task DeleteOrderFromDB(OrderModel order)
        {
            try
            {
                await ApiService.Instance.DeleteDataAsync(endpoint: ApiConstants.Orders, id: order.id, token: SecureTokenStorage.Instance.token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void AddOrderToDB(OrderModel order)
        {
            try 
            { 
                await ApiService.Instance.PostDataAsync(endpoint: ApiConstants.Orders, token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(order));

                // Actualiser la liste des produits après l'ajout
                await LoadOrdersAsync();
                MessageBox.Show("Commande ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateOrderInDB(OrderModel order)
        {
            try
            {
                await ApiService.Instance.PutDataAsync(endpoint: ApiConstants.Orders, token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(order));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
