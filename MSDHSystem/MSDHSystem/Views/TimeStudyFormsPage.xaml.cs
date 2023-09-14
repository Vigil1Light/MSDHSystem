using MSDHSystem.Models;
using MSDHSystem.ViewModels;
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
    public partial class TimeStudyFormsPage : ContentPage
    {
        public TimeStudyFormsPage()
        {
            InitializeComponent();
            this.BindingContext = new TimeStudyFormsViewModel();
        }
    }
}