using MSDHSystem.Models;
using MSDHSystem.Utils;
using MSDHSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSDHSystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LabTimeStudyPage : ContentPage
    {
        public LabTimeStudyPage()
        {
            InitializeComponent();
            this.BindingContext = new LabTimeStudyViewModel(lstLabTimeStudyDates);
        }
        private async void lstLabTimeStudyDates_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var selectedProduct = (TimeStudyDate)e.Item;
                Application.Current.Properties["TimeStudyDateValue"] = selectedProduct;
                if (selectedProduct.status == "(Waiting Approve)")
                {
                    DependencyService.Get<Toast>().Show("Lab Time Study Form is Waiting Approval you can’t click to Edit");
                }
                else
                {
                    IsLoading(true);
                    Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
                    newThread.Start();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message.ToString(), "Ok");
            }
            lstLabTimeStudyDates.SelectedItem = null;
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
                await Shell.Current.GoToAsync($"{nameof(LabTimeStudyFormsPage)}");
                IsLoading(false);
            });
        }

        void IsLoading(bool state)
        {
            lstLabTimeStudyDates.IsEnabled = !state;
            activity.IsRunning = state;
        }
    }
}