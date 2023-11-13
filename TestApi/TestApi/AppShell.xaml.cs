using System;
using System.Collections.Generic;
using TestApi.Pages.CartPage;
using TestApi.ViewModels;
using TestApi.Views;
using Xamarin.Forms;

namespace TestApi
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            

        }

    }
}
