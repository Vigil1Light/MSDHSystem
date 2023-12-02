using MSDHSystem.Models;
using MSDHSystem.Utils;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using MSDHSystem.Views;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

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
                UpdateUI(false);
                Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
                newThread.Start();
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
                    result = await app.AcquireTokenByUsernamePassword(scopes,
                                                                     username + @"@" + domain,
                                                                     password)
                                     .ExecuteAsync();
                    if (result != null)
                    {
                        UpdateUI(true);
                        string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                        string strQuery = string.Format("SELECT * FROM AD_Info WHERE Login_Name = '{0}'", Users.username);
                        SqlConnection con = new SqlConnection(connstring);
                        con.Open();
                        SqlCommand command = new SqlCommand(strQuery, con);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                await Xamarin.Essentials.SecureStorage.SetAsync("pid_number", reader["pid_nmbr"].ToString());
                                await Xamarin.Essentials.SecureStorage.SetAsync("pin_number", reader["pin_win_nmbr"].ToString());
                                await Xamarin.Essentials.SecureStorage.SetAsync("email", reader["email"].ToString());
                            }
                        }
                        reader.Close();
                        con.Close();
                        DependencyService.Get<Toast>().Show("Login success");
                        await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                        await Xamarin.Essentials.SecureStorage.SetAsync("username", Users.username);
                        await Xamarin.Essentials.SecureStorage.SetAsync("pass", Users.password);
                        Application.Current.MainPage = new AppShell(Users);
                        await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
                    }
                }
                catch (MsalServiceException ex)
                {
                    if (ex.Message.Contains("AADSTS50055"))
                    {
                        UpdateUI(true);
                        string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                        string strQuery = string.Format("SELECT * FROM AD_Info WHERE Login_Name = '{0}'", Users.username);
                        SqlConnection con = new SqlConnection(connstring);
                        con.Open();
                        SqlCommand command = new SqlCommand(strQuery, con);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                await Xamarin.Essentials.SecureStorage.SetAsync("pid_number", reader["pid_nmbr"].ToString());
                                await Xamarin.Essentials.SecureStorage.SetAsync("pin_number", reader["pin_win_nmbr"].ToString());
                                await Xamarin.Essentials.SecureStorage.SetAsync("email", reader["email"].ToString());
                            }
                        }
                        reader.Close();
                        con.Close();
                        DependencyService.Get<Toast>().Show("Login success");
                        await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                        await Xamarin.Essentials.SecureStorage.SetAsync("username", Users.username);
                        await Xamarin.Essentials.SecureStorage.SetAsync("pass", Users.password);
                        Application.Current.MainPage = new AppShell(Users);
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

        public void CheckSupervisor(string username)
        {
            string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
            string strQuery = string.Format("SELECT DISTINCT SupervisorName FROM TimeStudyDetail WHERE SupervisorName = '{0}'", username + "@msdh.ms.gov");
            SqlConnection con = new SqlConnection(connstring);
            con.Open();
            SqlCommand command = new SqlCommand(strQuery, con);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                Users.issupervisor = true;
            }
            else
            {
                Users.issupervisor = false;
            }
            reader.Close();
            con.Close();
        }

        private void LongRunningTask(object parameter)
        {
            // Get the parameter value
            string parameterValue = parameter as string;

            // Perform the long-running operation here
            // ...
            CheckSupervisor(Users.username);
            // Update UI from the background thread using Device.BeginInvokeOnMainThread
            Device.BeginInvokeOnMainThread( () =>
            {
                // Update UI to indicate that the operation has completed
                
            });
        }
    }
}
