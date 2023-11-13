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
                establishmentData.Fields.TryGetValue("from_time_formatted", out var fromTimeFormatted) &&
                establishmentData.Fields.TryGetValue("to_time_formatted", out var toTimeFormatted))
            {


                //Главный Контейнер куда записывается контент списка заведений
                Frame restaurantFrame = new Frame
                {
                    WidthRequest = 180,
                    Margin = 2,
                    Padding = 0,
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("#F4F4F4"),
                    HasShadow = false,
                };
                restaurantFrame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => HandleFrameTapped(establishmentData))
                });


                //Фото ресторана
                Image restaurantImage = new Image
                {
                    Source = "https://dostavka312.kg/public/public/photo/5ea59453011df.jpg", //путь к изображению
                    Aspect = Aspect.AspectFill,
                    
                };

                //Имя заведения
                Label restaurantNameLabel = new Label
                {
                    Text = establishmentName, // имя заведения
                    FontAttributes = FontAttributes.Bold, //Делаем текст жирным 
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    TextColor = Color.Black,
                    FontSize = 16,

                };

                


                //---------------------------------------------Контейнер для указания рейтинга заведения в ряд-----------------------------------------------------
                StackLayout stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal, // Расположение элементов в горизонтальном порядке
                    VerticalOptions = LayoutOptions.CenterAndExpand, // Выравнивание по вертикали
                    Spacing = 5 // Расстояние между элементами
                };

                //иконка звезды
                Image favoriteImage = new Image
                {
                    Source = "favorite.png",
                };


                //Текст рейтига 
                Label feedback = new Label
                {
                   Text = "4.7",
                   TextColor = Color.Black,
                   FontAttributes = FontAttributes.Bold

                };

                //Текст кол-ва комментариев
                Label feedBackCount = new Label
                {
                    Text = "(200+)",
                    TextColor = Color.FromHex("#828282")
                };

                //Соответвенно то, как это все добавляется в контейнер stackLayout
                stackLayout.Children.Add( favoriteImage);
                stackLayout.Children.Add( feedback);
                stackLayout.Children.Add( feedBackCount);



                //---------------------------------------------Контейнер для указания времени работы заведения в ряд-----------------------------------------------------
                StackLayout workTime = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal, // Расположение элементов в горизонтальном порядке
                    VerticalOptions = LayoutOptions.CenterAndExpand, // Выравнивание по вертикали
                    Spacing = 5 // Расстояние между элементами
                };

                Image workTimeIcon = new Image
                {
                    Source = "Clock.png"
                };

                Label workFrom = new Label
                {
                    Text = fromTimeFormatted // время начала работы
                };

                Label line = new Label
                {
                    Text = "-",
                };

                Label workUntill = new Label
                {
                    Text = toTimeFormatted // время конца работы
                };

                workTime.Children.Add( workTimeIcon);
                workTime.Children.Add( workFrom);
                workTime.Children.Add( line);
                workTime.Children.Add( workUntill);







                //---------------------------------------------Контейнер для указания типа заведения в ряд-----------------------------------------------------


                StackLayout kichenType = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal, // Расположение элементов в горизонтальном порядке
                    VerticalOptions = LayoutOptions.CenterAndExpand, // Выравнивание по вертикали
                    Spacing = 5 // Расстояние между элементами
                };

                Image forkIcon = new Image
                {
                    Source = "Fork.png"
                };

                //Название категории
                Label categoryNameLabel = new Label
                {
                    Text = categoryName, // название категории
                    FontAttributes = FontAttributes.Italic,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    TextColor = Color.Gray,
                    FontSize = 16,
                };

                kichenType.Children.Add( forkIcon );
                kichenType.Children.Add(categoryNameLabel);





                //--------------------------------------------- Контейнер для цены доставки и среднего чека в ряд -----------------------------------------------------


                StackLayout mainLastParams = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal, // Расположение элементов в горизонтальном порядке
                    VerticalOptions = LayoutOptions.CenterAndExpand, // Выравнивание по вертикали
                    Spacing = 5 // Расстояние между элементами
                };

                StackLayout deliveryCost = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal, // Расположение элементов в горизонтальном порядке
                    VerticalOptions = LayoutOptions.CenterAndExpand, // Выравнивание по вертикали
                    Spacing = 5 // Расстояние между элементами
                };

                Image truckIcon = new Image
                {
                    Source = "truck.png"
                };

                Label dostavkaNameLabel = new Label
                {
                    Text = price_dostavka + "сом", // название категории
                    FontAttributes = FontAttributes.Italic,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    TextColor = Color.Gray,
                    FontSize = 16
                };

                deliveryCost.Children.Add(truckIcon);
                deliveryCost.Children.Add(dostavkaNameLabel);



                StackLayout billCost = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal, // Расположение элементов в горизонтальном порядке
                    VerticalOptions = LayoutOptions.CenterAndExpand, // Выравнивание по вертикали
                    Spacing = 5 // Расстояние между элементами
                };

                Image billIcon = new Image
                {
                    Source = "bill.png"
                };

                Label billCountLabel = new Label
                {
                    Text = percentStavka + "сом", // название категории
                    FontAttributes = FontAttributes.Italic,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    TextColor = Color.Gray,
                    FontSize = 16
                };

                billCost.Children.Add(billIcon);
                billCost.Children.Add(billCountLabel);


                mainLastParams.Children.Add(deliveryCost);
                mainLastParams.Children.Add(billCost);


                restaurantFrame.Content = new StackLayout
                {
                    Children =
                    {
                        restaurantImage,
                        restaurantNameLabel,
                        stackLayout,
                        workTime,
                        kichenType,
                        mainLastParams
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