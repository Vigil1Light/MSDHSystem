using MSDHSystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSDHSystem.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private string headertext;

        public string HeaderText
        {
            get => headertext;
            set => SetProperty(ref headertext, value);
        }

        public AppShellViewModel(Users user)
        {
            HeaderText = "Welcome " + user.username;
        }
    }
}
