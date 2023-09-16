using dotMorten.Xamarin.Forms;
using MSDHSystem.Models;
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

        public TimeStudyFormsPage()
        {
            InitializeComponent();
            GetEmailValue();
            this.BindingContext = new TimeStudyFormsViewModel(lstTimeStudy, staticSuggestBox);
        }

        private void AutoSuggestBox_TextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            staticSuggestBox.ItemsSource = string.IsNullOrWhiteSpace(staticSuggestBox.Text)
                ? null
                : SupervisorEmailList.Where(filter => filter.StartsWith(staticSuggestBox.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
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
    }
}