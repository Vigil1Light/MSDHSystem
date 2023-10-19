using MSDHSystem.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSDHSystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeStudyReviewPage : ContentPage
    {
        public TimeStudyReviewPage()
        {
            InitializeComponent();
            this.BindingContext = new TimeStudyReviewViewModel();
        }
    }
}