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
        private bool istsupervisor = false;
        private bool islsupervisor = false;
        public string HeaderText
        {
            get => headertext;
            set => SetProperty(ref headertext, value);
        }
        public bool IsTSupervisor
        {
            get => istsupervisor;
            set => SetProperty(ref istsupervisor, value);
        }
        public bool IsLSupervisor
        {
            get => islsupervisor;
            set => SetProperty(ref islsupervisor, value);
        }

        public AppShellViewModel(Users user)
        {
            HeaderText = "Welcome " + user.username;
            IsTSupervisor = user.istsupervisor;
            IsLSupervisor = user.islsupervisor;
        }
    }
}
