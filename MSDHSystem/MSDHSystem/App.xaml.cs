using MSDHSystem.Models;
using MSDHSystem.Services;
using MSDHSystem.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSDHSystem
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

/*            DependencyService.Register<MockDataStore>();
            var isLoogged = Xamarin.Essentials.SecureStorage.GetAsync("isLogged").Result;
            var users = new Users();
            users.username = Xamarin.Essentials.SecureStorage.GetAsync("username").Result;
            if (isLoogged == "1")
            {
                MainPage = new AppShell(users);
            }
            else
            {
                MainPage = new LoginPage();
            }*/
            MainPage = new LoginPage();

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
