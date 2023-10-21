using dotMorten.Xamarin.Forms;
using MSDHSystem.Models;
using MSDHSystem.Utils;
using MSDHSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSDHSystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeStudyFormsPage : ContentPage
    {
        public List<string> SupervisorEmailList = new List<string>();
        bool bstate = false;
        public TimeStudyFormsPage()
        {
            InitializeComponent();
            GetEmailValue();
            this.BindingContext = new TimeStudyFormsViewModel(staticSuggestBox);
            SubmitBtn.IsEnabled = false;
        }

        private void AutoSuggestBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            staticSuggestBox.ItemsSource = string.IsNullOrWhiteSpace(staticSuggestBox.Text)
                ? null
                : SupervisorEmailList.Where(filter => filter.StartsWith(staticSuggestBox.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if(SupervisorEmailList.FirstOrDefault(s => s == staticSuggestBox.Text) != null)
            {
                bstate = true;
                Xamarin.Essentials.SecureStorage.SetAsync("submitstate", "1");
            }
            else
            {
                bstate= false;
                Xamarin.Essentials.SecureStorage.SetAsync("submitstate", "0");
            }
        }

        public void GetEmailValue()
        {
            string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
            string strQuery = string.Format("SELECT * FROM AD_Info");
            SqlConnection con = new SqlConnection(connstring);
            con.Open();
            SqlCommand command = new SqlCommand(strQuery, con);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    SupervisorEmailList.Add(reader["email"].ToString());
                }
            }
            reader.Close();
            con.Close();
        }

        private void AutoSuggestBox_QuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            if (e.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                if (((AutoSuggestBox)sender).Text == Xamarin.Essentials.SecureStorage.GetAsync("username").Result + "@msdh.ms.gov")
                {
                    DependencyService.Get<Toast>().Show("Can't send mail to yourself");
                    SubmitBtn.IsEnabled = false;
                }
                else
                {
                    SubmitBtn.IsEnabled = true;
                }

            }
            else
            {
                // User hit Enter from the search box. Use args.QueryText to determine what to do.
                if(!bstate)
                {
                    SubmitBtn.IsEnabled = false;
                    DependencyService.Get<Toast>().Show("Please select correct supervisor email");
                }
            }

        }
    }
}