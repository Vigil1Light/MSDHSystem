using MSDHSystem.Models;
using MSDHSystem.ViewModels;
using MSDHSystem.Views;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MSDHSystem
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell(Users users)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(TimeStudyPage), typeof(TimeStudyPage));
            this.BindingContext = new AppShellViewModel(users);
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        { 
            await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
            //await Shell.Current.GoToAsync("//LoginPage");
            App.Current.MainPage = new LoginPage();
        }

        private void OnHelpItem_Clicked(object sender, EventArgs e)
        {

        }
    }
}
