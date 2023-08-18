using MSDHSystem.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace MSDHSystem.Views
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