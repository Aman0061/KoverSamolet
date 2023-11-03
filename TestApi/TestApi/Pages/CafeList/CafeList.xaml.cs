using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using Xamarin.Forms;
using TestApi.Pages.PageCafe;
using static TestApi.Pages.CafeList.CafeList;
using static TestApi.Pages.TestApiPost.TestPost;
using System.Collections.Generic;

namespace TestApi.Pages.CafeList
{
    public partial class CafeList : ContentPage
    {
        public CafeList()
        {
            InitializeComponent();
            LoadDataAsync(); // Загружаем первую страницу блоков

        }

        private EstablishmentData establishmentData;

        private async void LoadDataAsync()
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "http://kover-site.333.kg/get_establishments";
                string response = await client.GetStringAsync(url);

                var json = JObject.Parse(response);
                JArray establishments = json["establishment"] as JArray;

                if (establishments != null && establishments.Count > 0)
                {
                    foreach (var establishment in establishments)
                    {
                        EstablishmentData establishmentData = ParseEstablishmentData(establishment);
                        CreateCityBlock(establishmentData);
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибки при загрузке данных
                Console.WriteLine("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private EstablishmentData ParseEstablishmentData(JToken establishment)
        {
            var establishmentData = new EstablishmentData();

            if (establishment is JObject establishmentObject)
            {
                foreach (var property in establishmentObject.Properties())
                {
                    string key = property.Name;
                    var value = property.Value;

                    if (value != null)
                    {
                        if (value.Type == JTokenType.String)
                        {
                            establishmentData.Fields[key] = value.Value<string>();
                        }
                        else
                        {
                            establishmentData.Fields[key] = value.ToString();
                        }
                    }

                    // Проверка, если ключ - 'time'
                    if (key == "time" && value is JArray timeArray)
                    {
                        // Обработка массива времени
                        foreach (var timeItem in timeArray)
                        {
                            if (timeItem is JObject timeObject)
                            {
                                if (timeObject.TryGetValue("from_time_formatted", out var fromTimeFormatted))
                                {
                                    establishmentData.Fields["from_time_formatted"] = fromTimeFormatted.ToString();
                                }
                                if (timeObject.TryGetValue("to_time_formatted", out var toTimeFormatted))
                                {
                                    establishmentData.Fields["to_time_formatted"] = toTimeFormatted.ToString();
                                }
                                // Обработай остальные поля времени если необходимо
                            }
                        }
                    }
                }
            }

            return establishmentData;
        }

        public class EstablishmentData
        {
            public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
        }

        private void CreateCityBlock(EstablishmentData establishmentData)
        {

            if(establishmentData.Fields.TryGetValue("establishment_name", out var establishmentName) &&
                establishmentData.Fields.TryGetValue("category_name", out var categoryName) &&
                establishmentData.Fields.TryGetValue("code_establishment", out var codeEstablishment) &&
                establishmentData.Fields.TryGetValue("price_dostavka", out var price_dostavka) &&
                establishmentData.Fields.TryGetValue("percent_stavka", out var percentStavka) &&
                establishmentData.Fields.TryGetValue("from_time_formatted", out var fromTimeFormatted))
            {
                Frame restaurantFrame = new Frame
                {
                    WidthRequest = 180,
                    Margin = 5,
                    Padding = 0,
                    CornerRadius = 10,
                };
                restaurantFrame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => HandleFrameTapped(establishmentData))
                });

                Image restaurantImage = new Image
                {
                    Source = "https://dostavka312.kg/public/public/photo/5ea59453011df.jpg", //путь к изображению
                    Aspect = Aspect.AspectFill,
                };

                Label restaurantNameLabel = new Label
                {
                    Text = establishmentName, // имя заведения
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    TextColor = Color.Black,
                    FontSize = 20,
                    Margin = new Thickness(5, 0, 0, 0),

                };

                Label restaurantCodeLabel = new Label
                {
                    Text = codeEstablishment, // код заведения
                    FontAttributes = FontAttributes.Italic,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    TextColor = Color.Gray,
                    FontSize = 16,
                    Margin = new Thickness(5, 0, 0, 0),

                };

                Label categoryNameLabel = new Label
                {
                    Text = categoryName, // название категории
                    FontAttributes = FontAttributes.Italic,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    TextColor = Color.Gray,
                    FontSize = 16,
                    Margin = new Thickness(5, 0, 0, 0),
                };

                Label dostavkaNameLabel = new Label
                {
                    Text = "Доставка: " + price_dostavka + "сом", // название категории
                    FontAttributes = FontAttributes.Italic,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    TextColor = Color.Gray,
                    Margin = new Thickness(5,0,0,0),

                    FontSize = 16
                };

                restaurantFrame.Content = new StackLayout
                {
                    Children =
                    {
                        restaurantImage,
                        restaurantNameLabel,
                        categoryNameLabel,
                        dostavkaNameLabel
                        // другие элементы, такие как рейтинг, время работы, кухня и др.
                    }
                };
                 containerFlexLayout.Children.Add(restaurantFrame);
            }
            

        }

        private async void HandleFrameTapped(EstablishmentData establishmentData)
        {
            if (establishmentData.Fields.TryGetValue("code_establishment", out var codeEstablishment))
            {
                using (var client = new HttpClient())
                {
                    var postData = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("code_establishment", codeEstablishment),
                        new KeyValuePair<string, string>("code_category", "0")
                    };

                    var content = new FormUrlEncodedContent(postData);
                    var response = await client.PostAsync("http://kover-site.333.kg/products", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        // Создайте новую страницу Page1, передавая establishmentData и данные JSON в конструктор
                        var page1 = new Page1(establishmentData, responseContent);
                        await Navigation.PushAsync(page1);
                    }
                    else
                    {
                        _ = DisplayAlert("Ошибка", "Не удалось выполнить запрос", "OK");
                    }
                }
            }
        }

    }
}