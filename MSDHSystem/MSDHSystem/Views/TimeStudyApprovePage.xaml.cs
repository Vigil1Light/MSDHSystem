using MSDHSystem.Models;
using MSDHSystem.Utils;
using MSDHSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSDHSystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeStudyApprovePage : ContentPage
    {
        public TimeStudyApprovePage()
        {
            InitializeComponent();
            this.BindingContext = new TimeStudyApproveViewModel();
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

        private void LongRunningTask(object parameter)
        {
            // Get the parameter value
            string parameterValue = parameter as string;

            // Perform the long-running operation here
            // ...

            // Update UI from the background thread using Device.BeginInvokeOnMainThread
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Update UI to indicate that the operation has completed
                await Shell.Current.GoToAsync($"{nameof(TimeStudyDetailPage)}");
                IsLoading(false);
            });
        }

        void IsLoading(bool state)
        {
            lstTimeStudyApprove.IsEnabled = !state;
            activity.IsRunning = state;
        }
    }
}