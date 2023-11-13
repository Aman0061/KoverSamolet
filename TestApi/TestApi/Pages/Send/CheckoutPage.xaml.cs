using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApi.Pages.Send
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckoutPage : ContentPage
    {
        public CheckoutPage()
        {
            InitializeComponent();

            UpdateTotalAmount();

        }

        public decimal GetTotalAmount()
        {
            // Получите экземпляр CartManager (возможно, у вас уже есть глобальный доступ к CartManager)
            CartManager cartManager = CartManager.GetInstance();

            // Вызовите метод CalculateTotalAmount, чтобы получить общую сумму
            decimal totalAmount = cartManager.CalculateTotalAmount();

            return totalAmount;
        }
        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Получите экземпляр CartManager
                CartManager cartManager = CartManager.GetInstance();

                // Получите список товаров в корзине
                var cartItems = cartManager.GetCartItems();

                // Соберите коды заведения в список
                var establishmentCodes = cartItems
                    .Select(item => int.Parse(item.EstablishmentCode))
                    .Distinct() // Убираем повторяющиеся коды
                    .Select(codeid => new { codeid });

                var productData = cartItems.Select(item => new
                {
                    code_estabs = int.Parse(item.EstablishmentCode),
                    codeid_products = int.Parse(item.FoodCodeid),
                    count = item.Quantity,
                    price = item.ProductPrice,
                    filial = 3,
                    count_posuda = 10
                }).ToArray();

                var data = new
                {
                    phone = phoneEntry.Text,
                    fio = fioEntry.Text,
                    zakaz_from_address = "FAIZA",
                    zakaz_to_address = addressEntry.Text,
                    zakaz_to_address_dop = "GGWP",
                    zakaz_comment = commentEntry.Text,
                    sdacha = sdachaEntry.Text,
                    dostavka = "200",
                    summ = GetTotalAmount(),
                    check_dostavka = 0,
                    type_oplata = 1,
                    type_zakaz = 1,
                    estab = establishmentCodes, // Список уникальных кодов заведений
                    product = productData // Список данных о продуктах
                };

                // Преобразуй JSON-объект в строку
                string jsonData = JsonConvert.SerializeObject(data);

                // Создайте HttpClient
                using (HttpClient client = new HttpClient())
                {
                    // Укажите URL сервера, куда нужно отправить запрос
                    string url = "http://kover-site.333.kg/create_zakaz";

                    // Создайте содержимое запроса
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Отправьте POST-запрос на сервер
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Запрос успешно отправлен, обработайте ответ сервера (если нужно)
                        string responseContent = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        // Произошла ошибка при отправке запроса
                        await DisplayAlert("Ошибка", "Произошла ошибка при отправке запроса", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", "Произошла ошибка: " + ex.Message, "OK");
            }
        }

        private void UpdateTotalAmount()
        {
            // В этом примере предположим, что у вас есть доступ к корзине CartManager
            decimal totalAmount = CartManager.GetInstance().CalculateTotalAmount();

            // Обновите Label с общей суммой заказа
            totalAmountLabel.Text = totalAmount.ToString("C"); // Форматируйте сумму как валюту
        }

    }
}