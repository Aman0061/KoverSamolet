using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApi.Pages;
using TestApi.Pages.Send;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace TestApi.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartPage : ContentPage
    {
        public CartPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Получите данные о корзине из CartManager
            var cartItems = CartManager.GetInstance().GetCartItems();

            // Привяжите данные к ListView
            cartListView.ItemsSource = cartItems;

            // Вычислите и отобразите общую стоимость
            decimal totalCost = cartItems.Sum(item => item.ProductPrice * item.Quantity);
            totalLabel.Text = "Итого: " + totalCost.ToString("0.00") + "сом";
        }

        // Обработчик нажатия на элемент в списке
        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is CartItem selectedItem)
            {
                // Вы можете добавить дополнительную логику обработки выбранного товара, если это необходимо.
            }
        }


        // Обработчик нажатия на кнопку "Оформить заказ"
        private async void OnCheckoutClicked(object sender, EventArgs e)
        {
            // В этом методе вы можете реализовать логику оформления заказа, отправки данных на сервер и т. д.
            await Navigation.PushModalAsync(new CheckoutPage());
        }

        private void DecreaseQuantityClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.CommandParameter is CartItem cartItem)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                        UpdateCart();
                    }
                }
            }
        }

        private void IncreaseQuantityClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.CommandParameter is CartItem cartItem)
                {
                    cartItem.Quantity++;
                    UpdateCart();
                }
            }
        }

        private void CalculateTotalCost()
        {
            var cartItems = CartManager.GetInstance().GetCartItems();
            decimal totalCost = cartItems.Sum(item => item.ProductPrice * item.Quantity);
            totalLabel.Text = "Итого: $" + totalCost.ToString("0.00");
        }

        private void UpdateCart()
        {
            cartListView.ItemsSource = null; // Очистите источник данных
            cartListView.ItemsSource = CartManager.GetInstance().GetCartItems(); // Обновите данные
            CalculateTotalCost(); // Пересчитайте общую стоимость
        }
    }
}