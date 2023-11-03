using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApi.Pages.TestApiPost
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestPost : ContentPage
    {
        public TestPost()
        {
            InitializeComponent();
        }

        private async void GetDataButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "http://kover-site.333.kg/get_establishments"; // Ваш URL

                // Логин и пароль для авторизации
                //string login = "regina";
                //string password = "123";

                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                  //  "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"))
                //);

                HttpResponseMessage response = await client.GetAsync(url);
                HttpStatusCode statusCode = response.StatusCode;


                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    var establishments = JsonConvert.DeserializeObject<EstablishmentResponse>(jsonContent);

                    // Ограничим количество заведений до первых 10
                    var establishmentNames = establishments.establishment
                        .Select(est => est.establishment_name)
                        .Take(10)
                        .ToList();

                    establishmentListView.ItemsSource = establishmentNames;
                }
                else
                {
                    if (statusCode == HttpStatusCode.Unauthorized)
                    {
                        // Ошибка аутентификации (неверные логин и/или пароль)
                        await DisplayAlert("Ошибка", "Неверные учетные данные.", "OK");
                    }
                    else if (statusCode == HttpStatusCode.NotFound)
                    {
                        // Страница не найдена
                        await DisplayAlert("Ошибка", "Страница не найдена.", "OK");
                    }
                    else if (statusCode == HttpStatusCode.InternalServerError)
                    {
                        // Внутренняя ошибка сервера
                        await DisplayAlert("Ошибка", "Внутренняя ошибка сервера.", "OK");
                    }
                    else
                    {
                        // Общее сообщение об ошибке
                        await DisplayAlert("Ошибка", "Ошибка загрузки данных.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
            }
        }

        public class Establishment
        {
            public string codeid { get; set; }
            public string code_category { get; set; }
            public string code_establishment { get; set; }
            public string establishment_name { get; set; }
            public string establishments_comment { get; set; }
            public int status { get; set; }
            public string category_name { get; set; }
            public int price_dostavka { get; set; }
            public int percent_stavka { get; set; }
            public string description { get; set; }
            public int sorting_codeid { get; set; }
            public int still { get; set; }
            public string url_site { get; set; }
            public string foto_estab { get; set; }
            public string url_categ { get; set; }
            public List<EstablishmentTime> time { get; set; }
        }

        public class EstablishmentTime
        {
            public string code_establishment { get; set; }
            public string filial_name { get; set; }
            public string from_time_formatted { get; set; }
            public string to_time_formatted { get; set; }
            public string code_filial { get; set; }
            public string foto { get; set; }
        }

        public class EstablishmentResponse
        {
            public int result { get; set; }
            public List<Establishment> establishment { get; set; }
        }
    }
}
