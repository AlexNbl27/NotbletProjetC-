using System.Windows;
using System.Windows.Controls;

namespace Notblet.Views
{
    public partial class MainApp : Page
    {
        public MainApp()
        {
            InitializeComponent();
            ContentFrame.Navigate(new Home());
        }

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var pageName = button?.Tag.ToString();

            switch (pageName)
            {
                case "Home":
                    ContentFrame.Navigate(new Home());
                    break;

                case "Products":
                    ContentFrame.Navigate(new Stock());
                    break;

                case "Categories":
                    ContentFrame.Navigate(new Categories());
                    break;

                case "Clients":
                    ContentFrame.Navigate(new Clients());
                    break;

                case "Orders":
                    ContentFrame.Navigate(new Orders());
                    break;

                case "Logout":
                    HandleLogout();
                    break;

                default:
                    break;
            }
        }

        private void HandleLogout()
        {
            MessageBox.Show("Déconnexion réussie !");
            // Rediriger vers la page de connexion
            NavigationService?.Navigate(new Login());   
            
        }
    }
}
