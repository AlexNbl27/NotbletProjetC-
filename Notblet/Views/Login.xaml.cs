using Newtonsoft.Json;
using Notblet.Constants;
using Notblet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {

            // Désactive le bouton pendant l'exécution de la requête
            LoginButton.IsEnabled = false;

            var apiService = ApiService.Instance;
            string endpoint = ApiConstants.Login;
            var loginData = new { username = Username.Text, password = Password.Password };

            try
            {
                string jsonData = JsonConvert.SerializeObject(loginData);
                string result = await apiService.PostDataAsync(endpoint, jsonData);
                SecureTokenStorage.Instance.SaveToken(result);
                this.NavigationService.Navigate(new MainApp());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Les informations de connexion sont incorrectes");
            }
            finally
            {
                LoginButton.IsEnabled = true;
            }
        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
