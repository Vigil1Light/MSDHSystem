﻿using MSDHSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSDHSystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LabTimeStudyReviewPage : ContentPage
    {
        public LabTimeStudyReviewPage()
        {
            InitializeComponent();
            this.BindingContext = new LabTimeStudyReviewViewModel();
        }
    }
}