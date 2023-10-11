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
	public partial class TimeStudyPage : ContentPage
	{
		public TimeStudyPage ()
		{
			InitializeComponent ();
            this.BindingContext = new TimeStudyViewModel(lstTimeStudyDates);
        }
        private async void lstTimeStudyDates_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var selectedProduct = (TimeStudyDate)e.Item;
                Application.Current.Properties["TimeStudyDateValue"] = selectedProduct;
                IsLoading(true);
                Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
                newThread.Start();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message.ToString(), "Ok");
            }
            lstTimeStudyDates.SelectedItem = null;
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
                await Shell.Current.GoToAsync($"{nameof(TimeStudyFormsPage)}");
                IsLoading(false);
            });
        }

        void IsLoading(bool state)
        {
            lstTimeStudyDates.IsEnabled = !state;
            activity.IsRunning = state;
        }
    }
}