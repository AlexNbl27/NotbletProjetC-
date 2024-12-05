using Notblet.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Notblet.Views
{
    public partial class OrderDialog : Window
    {
        public OrderModel Order { get; set; }
        public List<ClientModel> Clients { get; set; }
        public List<ProductModel> Products { get; set; }

        public OrderDialog(List<ClientModel> clients, List<ProductModel> products, OrderModel? order = null)
        {
            InitializeComponent();
            DataContext = this;
            Clients = clients;
            Products = products;
            order ??= new OrderModel
            {
                quantity = 10,
                order_date = DateTime.Now,
                status = "En cours",
                client = Clients.First(),
                client_id = Clients.First().id,
                product = Products.First(),
                product_id = Products.First().id
            };
            Order = order;
            OrderQuantityTextBox.Text = Order.quantity.ToString();
            OrderDatePicker.SelectedDate = Order.order_date;
            OrderStatusComboBox.SelectedValue = Order.status;

            // Remplir la liste déroulante des clients
            OrderClientComboBox.ItemsSource = Clients;
            OrderClientComboBox.DisplayMemberPath = "name";
            OrderClientComboBox.SelectedValuePath = "id";
            OrderClientComboBox.SelectedValue = Order.client_id;
            OrderProductComboBox.SelectedItem = Order.client;

            // Remplir la liste déroulante des produits
            OrderProductComboBox.ItemsSource = Products;
            OrderProductComboBox.DisplayMemberPath = "name";
            OrderProductComboBox.SelectedValuePath = "id";
            OrderProductComboBox.SelectedValue = Order.product_id;
            OrderClientComboBox.SelectedItem = Order.product;
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OrderQuantityTextBox.Text) || OrderClientComboBox.SelectedItem == null || OrderProductComboBox.SelectedItem == null || OrderStatusComboBox.SelectedItem == null || OrderDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Tous les champs sont obligatoires", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Récupérer les informations de la commande
            Order.quantity = int.TryParse(OrderQuantityTextBox.Text, out int quantity) ? quantity : 0;
            Order.order_date = OrderDatePicker.SelectedDate ?? DateTime.MinValue;
            Order.status = (OrderStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "En cours";
            Order.client = (ClientModel)OrderClientComboBox.SelectedItem;
            Order.client_id = Order.client.id;
            Order.product = (ProductModel)OrderProductComboBox.SelectedItem;
            Order.product_id = Order.product.id;

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
