using LiveCharts;
using Newtonsoft.Json;
using Notblet.Constants;
using Notblet.Models;
using System.Windows;
using System.Windows.Controls;
using LiveCharts.Wpf;
using System.Windows.Media;
using NLog;

namespace Notblet.Views
{
    /// <summary>
    /// Logique d'interaction pour DashboardPage.xaml
    /// </summary>
    public partial class Home : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger(); // Initialize NLog logger

        List<OrderModel> orders;
        List<ProductModel> products;

        public Home()
        {
            InitializeComponent();
            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                Logger.Info("Chargement des commandes commencé."); // Log message
                string token = SecureTokenStorage.Instance.token;
                string response = await ApiService.Instance.GetDataAsync(ApiConstants.Orders, token: token);
                orders = JsonConvert.DeserializeObject<List<OrderModel>>(response) ?? new List<OrderModel>();
                products = orders.Any() ? orders.Select(order => order.product).ToList() : new List<ProductModel>();

                decimal totalSales = 0;
                for (int i = 0; i < orders.Count; i++)
                {
                    totalSales += orders[i].product.price * orders[i].quantity;
                }
                TotalSales.Text = totalSales.ToString("C");

                if (products.Any())
                {
                    int BestSellerProductId = products.GroupBy(product => product.id).OrderByDescending(group => group.Count()).First().Key;
                    BestSellerProduct.Text = products.Find(product => product.id == BestSellerProductId).name;
                    CreateChart();
                }

                Logger.Info("Chargement des commandes terminé avec succès."); // Log success
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Erreur lors du chargement des commandes."); // Log error
                MessageBox.Show($"Erreur lors du chargement des commandes : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateChart()
        {
            Logger.Info("Création du graphique des produits.");

            ProductsChart.Series.Clear();
            ProductsChart.AxisX.Clear();
            ProductsChart.AxisY.Clear();

            // Vérifier que 'orders' contient des données
            if (orders == null || !orders.Any())
            {
                Logger.Warn("Aucune commande disponible pour le graphique."); // Log warning
                MessageBox.Show("Aucune commande à afficher dans le graphique.", "Données manquantes", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Préparer les données pour le graphique
            var groupedData = orders.GroupBy(o => o.product.category_id)
                                    .Select(g => new
                                    {
                                        CategoryId = g.Key,
                                        CategoryName = g.First().product.category.name, // Assurez-vous d'avoir accès à 'name' de la catégorie
                                        TotalQuantity = g.Sum(o => o.quantity) // Somme des quantités des commandes
                                    })
                                    .ToList();

            // Ajouter les séries au graphique avec des couleurs personnalisées
            ProductsChart.Series.Add(new ColumnSeries
            {
                Title = "Produits par Catégorie",
                Values = new ChartValues<int>(groupedData.Select(g => g.TotalQuantity)),
                Fill = new SolidColorBrush(Color.FromRgb(255, 87, 34)), // Exemple de couleur personnalisée
                StrokeThickness = 1 // Bordure des barres
            });

            // Ajouter les axes
            ProductsChart.AxisX.Add(new Axis
            {
                Title = "Catégories",
                Labels = groupedData.Select(g => g.CategoryName).ToArray(), // Ajouter les labels des catégories
                LabelFormatter = value => value.ToString("N0"), // Formatage des labels
                Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 1 }, // Ajout d'une séparation fine
                FontSize = 12 // Taille de la police des labels
            });

            ProductsChart.AxisY.Add(new Axis
            {
                Title = "Quantité Totale",
                LabelFormatter = value => value.ToString("N0"), // Formater les valeurs de l'axe Y avec des séparateurs de milliers
                FontSize = 12 // Taille de la police des labels
            });

            Logger.Info("Graphique des produits créé avec succès.");
        }
    }
}
