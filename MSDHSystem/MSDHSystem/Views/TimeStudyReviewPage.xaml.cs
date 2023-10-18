using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.BindingContext = new TimeStudyReviewPage();
        }
    }
}