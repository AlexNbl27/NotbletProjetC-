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
    public partial class Clients : Page
    {
        public Clients()
        {
            InitializeComponent();
            LoadClientsAsync();
        }

        private async Task LoadClientsAsync()
        {
            try
            {
                string response = await ApiService.Instance.GetDataAsync(endpoint: ApiConstants.Clients, token: SecureTokenStorage.Instance.token);
                List<ClientModel> clients = JsonConvert.DeserializeObject<List<ClientModel>>(response) ?? [];
                if (clients.Count > 0)
                {
                    ClientsDataGrid.ItemsSource = clients;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des clients : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            var addClientDialog = new ClientDialog()
            {
                Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre modale
            };

            if (addClientDialog.ShowDialog() == true)
            {
                AddClientToDB(addClientDialog.Client);
            }
        }

        private void ClientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem is ClientModel selectedClient)
            {
                var editClientDialog = new ClientDialog(client: selectedClient)
                {
                    Owner = Window.GetWindow(this) // Définit le propriétaire de la fenêtre
                };

                if (editClientDialog.ShowDialog() == true)
                {
                    UpdateClientInDB(editClientDialog.Client);
                }
            }
        }

        private async void AddClientToDB(ClientModel client)
        {
            try
            {
                await ApiService.Instance.PostDataAsync(endpoint: ApiConstants.Clients, token: SecureTokenStorage.Instance.token, jsonData: JsonConvert.SerializeObject(client));

                // Actualiser la liste des clients après l'ajout
                await LoadClientsAsync();
                MessageBox.Show("Client ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateClientInDB(ClientModel client)
        {
            try
            {
                await ApiService.Instance.PutDataAsync(
                                       endpoint: $"{ApiConstants.Clients}/{client.id}",
                                                          token: SecureTokenStorage.Instance.token,
                                                                             jsonData: JsonConvert.SerializeObject(client)
                                                                                            );

                // Actualiser la liste des produits après la modification
                await LoadClientsAsync();
                MessageBox.Show("Produit modifié avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification du produit : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteSelectedClientsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un client à supprimer.", "Sélection manquante", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Voulez-vous vraiment supprimer le(s) client(s) sélectionné(s) ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var client in ClientsDataGrid.SelectedItems)
                    {
                        await DeleteClientFromDB(client as ClientModel);
                    }

                    // Actualiser la liste des clients après suppression
                    await LoadClientsAsync();
                    MessageBox.Show("Client(s) supprimé(s) avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la suppression du client : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task DeleteClientFromDB(ClientModel client)
        {
            try
            {
                await ApiService.Instance.DeleteDataAsync(
                                                          endpoint: ApiConstants.Clients, id: client.id,
                                                                                                                   token: SecureTokenStorage.Instance.token
                                                                                                                                                                                           );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la suppression du client : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
