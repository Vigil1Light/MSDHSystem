using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSDHSystem.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TogglePasswordEntry : ContentView
    {
        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(TogglePasswordEntry));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(TogglePasswordEntry),
                defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty HidePasswordProperty =
            BindableProperty.Create(nameof(HidePassword), typeof(bool), typeof(TogglePasswordEntry),
                defaultValue: true);

        public static readonly BindableProperty HidePasswordColorProperty =
            BindableProperty.Create(nameof(HidePasswordColor), typeof(Color), typeof(TogglePasswordEntry),
                defaultValue: Color.Black);

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool HidePassword
        {
            get => (bool)GetValue(HidePasswordProperty);
            set => SetValue(HidePasswordProperty, value);
        }

        public Color HidePasswordColor
        {
            get => (Color)GetValue(HidePasswordColorProperty);
            set => SetValue(HidePasswordColorProperty, value);
        }

        public TogglePasswordEntry()
        {
            InitializeComponent();
        }

        private void OnImageButtonClicked(object sender, EventArgs e)
        {
            HidePassword = !HidePassword;
        }
    }
}