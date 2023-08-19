using MSDHSystem.Models;
using MSDHSystem.Utils;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MSDHSystem.Views;

namespace MSDHSystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        private Users users { get; set; }
        private bool isloading = false;
        private bool isenabled = true;

        public Command LoginCommand { get; }


        public Users Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => isloading;
            set => SetProperty(ref isloading, value);
        }

        public bool IsEnabled
        {
            get => isenabled;
            set => SetProperty(ref isenabled, value);
        }

        public LoginViewModel()
        {
            Users = new Users();
            LoginCommand = new Command(OnLoginClicked);
        }

        private void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            if (Users.username == null || Users.password == null)
            {
                DependencyService.Get<Toast>().Show("Please input Username and Password");
            }
            else
            {
                _ = ADlogin(Users.username, Users.password);
            }
        }

        private async Task ADlogin(string username, string password)
        {
            string domain = @"luling626167outlook.onmicrosoft.com";
            string authority = "https://login.microsoftonline.com/" + domain;
            string[] scopes = new string[] { "user.read" };
            string clientId = "00801c0c-ec28-490c-9e54-56fba8047fa8";
            IPublicClientApplication app;
            app = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithIosKeychainSecurityGroup(Xamarin.Essentials.AppInfo.PackageName)
                .Build();
            var accounts = await app.GetAccountsAsync();

            AuthenticationResult result = null;
            if (accounts.Any())
            {
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                  .ExecuteAsync();
            }
            else
            {
                try
                {
                    UpdateUI(false);
                    result = await app.AcquireTokenByUsernamePassword(scopes,
                                                                     username + @"@" + domain,
                                                                     password)
                                     .ExecuteAsync();
                    if (result != null)
                    {
                        UpdateUI(true);
                        DependencyService.Get<Toast>().Show("Login success");
                        await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                        Application.Current.MainPage = new AppShell();
                        await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                    }
                }
                catch (MsalServiceException ex)
                {
                    if (ex.Message.Contains("AADSTS50055"))
                    {
                        UpdateUI(true);
                        DependencyService.Get<Toast>().Show("Login success");
                        await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                        Application.Current.MainPage = new AppShell();
                        await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                    }
                    else
                    {
                        UpdateUI(true);
                        DependencyService.Get<Toast>().Show("Username or Password is wrong. Try again");
                    }
                }
            }
            Console.WriteLine(result.Account.Username);
        }

        public void UpdateUI(bool state)
        {

            IsEnabled = state;
            IsLoading = !state;
        }
    }
}
