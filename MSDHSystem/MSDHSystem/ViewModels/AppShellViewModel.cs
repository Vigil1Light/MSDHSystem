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
            IsSupervisor = user.issupervisor;
        }
    }
}
