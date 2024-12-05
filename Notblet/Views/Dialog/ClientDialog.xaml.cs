using Notblet.Models;
using System;
using System.Windows;

namespace Notblet.Views
{
    public partial class ClientDialog : Window
    {
        public ClientModel Client { get; set; }

        public ClientDialog(ClientModel? client = null)
        {
            InitializeComponent();

            client ??= new ClientModel{name = "", address = "",  siret = ""};
            Client = client;
            ClientNameTextBox.Text = Client.name;
            ClientAddressTextBox.Text = Client.address;
            ClientSiretTextBox.Text = Client.siret;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(ClientNameTextBox.Text) || string.IsNullOrWhiteSpace(ClientAddressTextBox.Text) || string.IsNullOrWhiteSpace(ClientSiretTextBox.Text))
            {
                MessageBox.Show("Tous les champs sont obligatoires", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Récupérer les informations du client à partir des TextBox et les affecter à l'objet Client
            Client.name = ClientNameTextBox.Text;
            Client.address = ClientAddressTextBox.Text;
            Client.siret = ClientSiretTextBox.Text;
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
