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
    public partial class LabTimeStudyApprovePage : ContentPage
    {
        public LabTimeStudyApprovePage()
        {
            InitializeComponent();
            this.BindingContext = new LabTimeStudyApproveViewModel();
        }
        private async void lstTimeStudyApprove_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            /*try
            {
                var selectedProduct = (TimeStudyApproveData)e.Item;
                Application.Current.Properties["TimeStudyDetailValue"] = selectedProduct;

                DependencyService.Get<Toast>().Show(selectedProduct.Name);
                IsLoading(true);
                Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
                newThread.Start();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message.ToString(), "Ok");
            }
            lstTimeStudyApprove.SelectedItem = null;*/
        }
    }
}