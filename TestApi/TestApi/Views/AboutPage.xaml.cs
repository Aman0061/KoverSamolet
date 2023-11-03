using System;
using System.ComponentModel;
using TestApi.Pages.CafeList;
using TestApi.Pages.CartPage;
using TestApi.Pages.PageCafe;
using TestApi.Pages.TestApiPost;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApi.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            
        }
        async void OnFrameTapped(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new CafeList());

        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage());

        }

    }
}