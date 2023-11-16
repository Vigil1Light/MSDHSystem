using MSDHSystem.Models;
using MSDHSystem.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Xamarin.Essentials;

namespace MSDHSystem.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private string headertext;
        private bool issupervisor = false;
        public string HeaderText
        {
            get => headertext;
            set => SetProperty(ref headertext, value);
        }
        public bool IsSupervisor
        {
            get => issupervisor;
            set => SetProperty(ref issupervisor, value);
        }

        public AppShellViewModel(Users user)
        {
            HeaderText = "Welcome " + user.username;
            CheckSupervisor(user);
        }

        public void CheckSupervisor(Users user)
        {
            string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
            string strQuery = string.Format("SELECT DISTINCT SupervisorName FROM TimeStudyDetail WHERE SupervisorName = '{0}'", user.username + "@msdh.ms.gov");
            SqlConnection con = new SqlConnection(connstring);
            con.Open();
            SqlCommand command = new SqlCommand(strQuery, con);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                IsSupervisor = true;
            }
            else
            {
                IsSupervisor = false;
            }
            reader.Close();
            con.Close();
        }
    }
}
