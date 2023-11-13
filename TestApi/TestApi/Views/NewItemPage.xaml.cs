using System;
using System.Collections.Generic;
using System.ComponentModel;
using TestApi.Models;
using TestApi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApi.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            Shell.SetNavBarIsVisible(this, false);
            BindingContext = new NewItemViewModel();
        }
    }
}