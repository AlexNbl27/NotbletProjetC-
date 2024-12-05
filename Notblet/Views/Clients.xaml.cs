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
    /// Logique d'interaction pour Clients.xaml
    /// </summary>
    public partial class Clients : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); // Create a logger instance

        public Clients()
        {
            InitializeComponent();
            Logger.Info("Page 'Clients' initialized.");
            LoadClientsAsync();
        }

        private async Task LoadClientsAsync()
        {
            try
            {
                Logger.Info("Loading clients...");
                string response = await ApiService.Instance.GetDataAsync(endpoint: ApiConstants.Clients, token: SecureTokenStorage.Instance.token);
                List<ClientModel> clients = JsonConvert.DeserializeObject<List<ClientModel>>(response) ?? new List<ClientModel>();
                if (clients.Count > 0)
                {
                    ClientsDataGrid.ItemsSource = clients;
                    Logger.Info($"{clients.Count} clients loaded successfully.");
                }
                else
                {
                    Logger.Warn("No clients found.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error loading clients");
                MessageBox.Show($"Erreur lors du chargement des clients : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info("Opening dialog to add a new client.");
            var addClientDialog = new ClientDialog()
            {
                Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre modale
            };

            if (addClientDialog.ShowDialog() == true)
            {
                Logger.Info("Client dialog returned with result 'true'. Adding client.");
                AddClientToDB(addClientDialog.Client);
            }
            else
            {
                Logger.Info("Client dialog was cancelled.");
            }
        }

        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is ClientModel selectedClient)
            {
                Logger.Info($"Editing client with ID: {selectedClient.id}");
                var editClientDialog = new ClientDialog(client: selectedClient)
                {
                    Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre
                };

                if (editClientDialog.ShowDialog() == true)
                {
                    Logger.Info($"Client with ID: {selectedClient.id} updated.");
                    UpdateClientInDB(editClientDialog.Client);
                }
            }
        }

        private async void AddClientToDB(ClientModel client)
        {
            try
            {
                Logger.Info("Adding new client to database...");
                await ApiService.Instance.PostDataAsync(endpoint: ApiConstants.Clients, token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(client));
                await LoadClientsAsync();
                Logger.Info("Client added successfully.");
                MessageBox.Show("Client ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error adding client");
                MessageBox.Show($"Erreur lors de l'ajout du client : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateClientInDB(ClientModel client)
        {
            try
            {
                Logger.Info($"Updating client with ID: {client.id} in database...");
                await ApiService.Instance.PutDataAsync(endpoint: $"{ApiConstants.Clients}/{client.id}", token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(client));
                await LoadClientsAsync();
                Logger.Info($"Client with ID: {client.id} updated successfully.");
                MessageBox.Show("Produit modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error updating client with ID: {client.id}");
                MessageBox.Show($"Erreur lors de la modification du client : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteSelectedClientsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItems.Count == 0)
            {
                Logger.Warn("No clients selected for deletion.");
                MessageBox.Show("Veuillez sélectionner un client à supprimer.", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Voulez-vous vraiment supprimer le(s) client(s) sélectionné(s) ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Logger.Info("Deleting selected client(s)...");
                    foreach (var client in ClientsDataGrid.SelectedItems)
                    {
                        await DeleteClientFromDB(client as ClientModel);
                    }

                    await LoadClientsAsync();
                    Logger.Info("Client(s) deleted successfully.");
                    MessageBox.Show("Client(s) supprimé(s) avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error deleting client(s)");
                    MessageBox.Show($"Erreur lors de la suppression du client : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task DeleteClientFromDB(ClientModel client)
        {
            try
            {
                Logger.Info($"Deleting client with ID: {client.id} from database...");
                await ApiService.Instance.DeleteDataAsync(endpoint: ApiConstants.Clients, id: client.id, token: SecureTokenStorage.Instance.token);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error deleting client with ID: {client.id}");
                MessageBox.Show($"Erreur lors de la suppression du client : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
