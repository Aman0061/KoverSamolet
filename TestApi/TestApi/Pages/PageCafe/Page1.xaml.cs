using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using static TestApi.Pages.CafeList.CafeList;

namespace TestApi.Pages.PageCafe
{
    public partial class Page1 : ContentPage
    {
        private string jsonData;
        private List<Button> categoryButtonsList;

        public Page1(EstablishmentData establishmentData, string jsonData)
        {
            InitializeComponent();
            this.jsonData = jsonData;
            categoryButtonsList = new List<Button>();

            cafeName.Text = establishmentData.Fields["establishment_name"];
            DeliveryPrice.Text = establishmentData.Fields["price_dostavka"].ToString() + " сом";
            StavkaPrice.Text = "~" + establishmentData.Fields["percent_stavka"].ToString() + " сом";
            WorkTime.Text = establishmentData.Fields["from_time_formatted"].ToString() + " - " + establishmentData.Fields["to_time_formatted"].ToString();

            // Отображаем код заведения
            establishmentCodeLabel.Text = establishmentData.Fields["code_establishment"].ToString();

            try
            {
                var json = JObject.Parse(jsonData);

                if (json["result"] != null && (int)json["result"] == 1)
                {
                    var products = json["product"] as JArray;

                    if (products != null)
                    {
                        var firstCategory = string.Empty;
                        foreach (var product in products)
                        {
                            var categoryName = (string)product["category_name"];
                            var estabArray = product["estab"] as JArray;

                            if (estabArray != null)
                            {
                                if (string.IsNullOrEmpty(firstCategory))
                                {
                                    firstCategory = categoryName;
                                    CategoryName.Text = firstCategory;
                                    foreach (var estab in estabArray)
                                    {
                                        var productName = (string)estab["product_name"];
                                        var productPrice = (string)estab["product_price"];
                                        var establishmentCode = (string)estab["code_establishment"]; // Добавляем establishment_code
                                        var foodcode = (string)estab["codeid"]; // Добавляем codeid


                                        AddFoodBlock(productName, productPrice, establishmentCode, foodcode);
                                    }
                                }

                                var categoryButton = CreateCategoryButton(categoryName);
                                categoryButtonsList.Add(categoryButton);
                                categoryButtons.Children.Add(categoryButton);
                            }
                        }
                    }
                    else
                    {
                        DisplayAlert("Error", "Missing", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при парсинге JSON: " + ex.Message);
            }

            // Устанавливаем цвет фона только для первой кнопки
            if (categoryButtonsList.Count > 0)
            {
                categoryButtonsList[0].BackgroundColor = Color.FromHex("#FFC12E");
            }
        }

        private Button CreateCategoryButton(string categoryName)
        {
            var categoryButton = new Button
            {
                Text = categoryName,
                CornerRadius = 10,
                TextColor = Color.Black,
                ClassId = categoryName,
                HeightRequest = 40,
                BackgroundColor = Color.Transparent,
            };

            categoryButton.Clicked += CategoryButton_Clicked;

            return categoryButton;
        }

        private Button selectedCategoryButton;
        private void CategoryButton_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string categoryName = button.ClassId;
                foodNameStackLayout.Children.Clear();

                var json = JObject.Parse(jsonData);
                var products = json["product"] as JArray;

                foreach (var product in products)
                {
                    if ((string)product["category_name"] == categoryName)
                    {
                        var estabArray = product["estab"] as JArray;

                        if (estabArray != null)
                        {
                            foreach (var estab in estabArray)
                            {
                                var productName = (string)estab["product_name"];
                                var productPrice = (string)estab["product_price"];
                                var establishmentCode = (string)estab["code_establishment"]; // Добавляем establishment_code
                                var foodcode = (string)estab["codeid"]; // Добавляем codeid

                                AddFoodBlock(productName, productPrice, establishmentCode, foodcode); // Передаем establishment_code
                            }
                        }
                    }
                }

                CategoryName.Text = categoryName;

                // Сбрасываем цвет фона у всех кнопок
                foreach (var categoryButton in categoryButtonsList)
                {
                    categoryButton.BackgroundColor = Color.Transparent;
                }

                // Устанавливаем цвет фона для выбранной кнопки
                button.BackgroundColor = Color.FromHex("#FFC12E");
            }
        }



        private void AddFoodBlock(string productName, string productPrice, string establishmentCode, string foodcode)
        {
            var foodBlock = new StackLayout
            {
                Orientation = StackOrientation.Vertical
            };

            var foodNameLabel = new Label
            {
                Text = productName,
                TextColor = Color.FromHex("#222"),
                FontSize = 16
            };

            var productPriceLabel = new Label
            {
                Text = productPrice + " сом",
                TextColor = Color.Black,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold
            };

            var codeid = new Label
            {
                Text = foodcode,
            };

            var establishmentCodeLabel = new Label
            {
                Text = "Код заведения: " + establishmentCode, // Отобразите код заведения
                TextColor = Color.FromHex("#777"),
                FontSize = 14
            };

            var addToCartButton = new Button
            {
                Text = "В корзину",
                BackgroundColor = Color.FromHex("#FFC12E"),
                TextColor = Color.Black,
                CornerRadius = 5,
                HeightRequest = 35
            };
            addToCartButton.Clicked += (sender, e) =>
            {
                // Создайте объект CartItem на основе выбранного блюда, establishmentCode и добавьте его в корзину.
                var cartItem = new CartItem
                {
                    ProductName = productName,
                    ProductPrice = decimal.Parse(productPrice),
                    EstablishmentCode = establishmentCode, // Установите establishmentCode
                    FoodCodeid = foodcode,
                    Quantity = 1 // Начальное количество
                };

                CartManager.GetInstance().AddToCart(cartItem);

                // Оповестите пользователя о добавлении в корзину, например, через всплывающее сообщение.
                DisplayAlert("Добавлено в корзину", productName + " добавлено в корзину", "OK");
            };

            foodBlock.Children.Add(foodNameLabel);
            foodBlock.Children.Add(productPriceLabel);
            foodBlock.Children.Add(establishmentCodeLabel); // Добавьте код заведения в блок
            foodBlock.Children.Add(addToCartButton);
            foodBlock.Children.Add(codeid);

            foodNameStackLayout.Children.Add(foodBlock);
        }


        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue; // Получаем текст из поля поиска
            PerformSearch(searchText); // Выполняем поиск
        }

        private void PerformSearch(string searchText)
        {
            // Очистите отображаемые блоки с блюдами
            foodNameStackLayout.Children.Clear();

            // Преобразуйте текст поиска в нижний регистр
            searchText = searchText.ToLower();

            var json = JObject.Parse(jsonData);
            var products = json["product"] as JArray;

            foreach (var product in products)
            {
                var estabArray = product["estab"] as JArray;

                if (estabArray != null)
                {
                    foreach (var estab in estabArray)
                    {
                        var productName = (string)estab["product_name"];
                        var productPrice = (string)estab["product_price"];
                        var establishmentCode = (string)estab["code_establishment"];
                        var foodcode = (string)estab["codeid"]; // Добавляем codeid


                        // Преобразуйте название продукта в нижний регистр
                        var productNameLower = productName.ToLower();

                        // Если текст поиска пустой или блюдо содержит текст поиска (регистронезависимо), отобразите его
                        if (string.IsNullOrEmpty(searchText) || productNameLower.Contains(searchText))
                        {
                            AddFoodBlock(productName, productPrice, establishmentCode, foodcode);
                        }
                    }
                }
            }
        }

    }
}
