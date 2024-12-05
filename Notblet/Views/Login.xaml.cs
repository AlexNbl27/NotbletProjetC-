using Newtonsoft.Json;
using Notblet.Constants;
using System.Windows;
using System.Windows.Controls;
using NLog;

namespace Notblet.Views
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        // Logger pour la classe Login
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                Logger.Info("Tentative de connexion avec le nom d'utilisateur : {0}", Username.Text);

                // Sérialiser les données de connexion
                string jsonData = JsonConvert.SerializeObject(loginData);
                Logger.Debug("Données envoyées pour la connexion : {0}", jsonData);

                // Appel API pour authentification
                string result = await apiService.PostDataAsync(endpoint, jsonData);
                Logger.Info("Réponse de l'API reçue : {0}", result);

                // Sauvegarder le token et naviguer vers l'application principale
                SecureTokenStorage.Instance.SaveToken(result);
                Logger.Info("Token sauvegardé avec succès.");

                // Navigation vers la page principale
                this.NavigationService.Navigate(new MainApp());
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors de la tentative de connexion.");
                MessageBox.Show("Les informations de connexion sont incorrectes");
            }
            finally
            {
                // Réactive le bouton après l'exécution
                LoginButton.IsEnabled = true;
            }
        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
