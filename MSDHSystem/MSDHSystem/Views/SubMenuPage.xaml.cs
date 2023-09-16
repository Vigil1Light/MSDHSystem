using Microsoft.SqlServer.Server;
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
	public partial class SubMenuPage : ContentPage
	{
        public SubMenuPage()
        {
            InitializeComponent();
            this.BindingContext = new SubMenuViewModel(Shell.Current.CurrentItem.Route, lstSubMenus);   
        }

        private async void lstSubMenus_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            try
            {
                var selectedProduct = (Menus)e.Item;
                if (selectedProduct.menuTitle == "Time Study")
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
            lstSubMenus.SelectedItem = null;
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
                await Shell.Current.GoToAsync(nameof(TimeStudyPage));
                IsLoading(false);
            });
        }

        void IsLoading(bool state)
        {
            lstSubMenus.IsEnabled = !state;
            activity.IsRunning = state;
        }
    }
}