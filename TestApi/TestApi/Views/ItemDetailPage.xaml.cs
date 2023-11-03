using System.ComponentModel;
using TestApi.ViewModels;
using Xamarin.Forms;

namespace TestApi.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}