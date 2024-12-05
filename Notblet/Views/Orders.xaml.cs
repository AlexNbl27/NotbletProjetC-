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
    /// Logique d'interaction pour Clients.xaml
    /// </summary>
    public partial class Orders : Page
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger(); // Initialize NLog logger

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
                Clients = JsonConvert.DeserializeObject<List<ClientModel>>(response) ?? new List<ClientModel>();
                logger.Info("Clients loaded successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while loading clients");
                MessageBox.Show($"Erreur lors du chargement des catégories : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadProductsAsync()
        {
            try
            {
                string token = SecureTokenStorage.Instance.token;
                string response = await ApiService.Instance.GetDataAsync(ApiConstants.Products, token: token);
                Products = JsonConvert.DeserializeObject<List<ProductModel>>(response) ?? new List<ProductModel>();
                logger.Info("Products loaded successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while loading products");
                MessageBox.Show($"Erreur lors du chargement des produits : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                string token = SecureTokenStorage.Instance.token;
                string response = await ApiService.Instance.GetDataAsync(ApiConstants.Orders, token: token);
                List<OrderModel> orders = JsonConvert.DeserializeObject<List<OrderModel>>(response) ?? new List<OrderModel>();
                if (orders.Count > 0)
                {
                    OrdersDataGrid.ItemsSource = orders;
                    logger.Info("Orders loaded successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while loading orders");
                MessageBox.Show($"Erreur lors du chargement des produits : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (Clients == null || Clients.Count == 0)
            {
                logger.Warn("No clients found when trying to add an order.");
                MessageBox.Show("Veuillez ajouter des clients avant de passer une commande.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Products == null || Products.Count == 0)
            {
                logger.Warn("No products found when trying to add an order.");
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
                logger.Info("Order added.");
            }
        }

        private void OrdersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is OrderModel selectedOrder)
            {
                var editOrderDialog = new OrderDialog(order: selectedOrder, clients: Clients, products: Products)
                {
                    Owner = Window.GetWindow(this)
                };

                if (editOrderDialog.ShowDialog() == true)
                {
                    UpdateOrderInDB(editOrderDialog.Order);
                    logger.Info($"Order {selectedOrder.id} updated.");
                }
            }
        }

        private async void DeleteSelectedOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrders = OrdersDataGrid.SelectedItems.Cast<OrderModel>().ToList();

            if (selectedOrders.Count == 0)
            {
                logger.Warn("No orders selected for deletion.");
                MessageBox.Show("Aucune commande sélectionnée pour suppression.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
                        logger.Info($"Order {order.id} deleted.");
                    }

                    await LoadOrdersAsync();
                    MessageBox.Show("Commandes supprimées avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error while deleting orders.");
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
                logger.Error(ex, "Error while deleting order from DB.");
                MessageBox.Show($"Erreur lors de la suppression de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddOrderToDB(OrderModel order)
        {
            try
            {
                await ApiService.Instance.PostDataAsync(endpoint: ApiConstants.Orders, token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(order));

                await LoadOrdersAsync();
                logger.Info("New order added to DB.");
                MessageBox.Show("Commande ajoutée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while adding order to DB.");
                MessageBox.Show($"Erreur lors de l'ajout de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateOrderInDB(OrderModel order)
        {
            try
            {
                await ApiService.Instance.PutDataAsync(endpoint: ApiConstants.Orders, token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(order));
                logger.Info($"Order {order.id} updated in DB.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while updating order in DB.");
                MessageBox.Show($"Erreur lors de la modification de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
