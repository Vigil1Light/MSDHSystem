using MSDHSystem.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MSDHSystem.Models
{
    public class TimeStudyData : INotifyPropertyChanged
    {
        public int No { get; set; }
        public string Program { get; set; }
        public string Activity { get; set; }
        public List<string> Programs { get; set;}
        public List<string> Activities { get; set;}

        private string h1 = "0";
        private string h2 = "0";
        private string h3 = "0";
        private string h4 = "0";
        private string h5 = "0";
        private string h6 = "0";
        private string h7 = "0";
        private string m1 = "0";
        private string m2 = "0";
        private string m3 = "0";
        private string m4 = "0";
        private string m5 = "0";
        private string m6 = "0";
        private string m7 = "0";
        private string totalHours { get; set; }
        private string totalMins { get; set; }

        public string TotalHours
        {
            get { return totalHours; }
            set
            {
                totalHours = value;
                OnPropertyChanged();
            }
        }

        public string TotalMins
        {
            get { return totalMins; }
            set
            {
                totalMins = value;
                OnPropertyChanged();
            }
        }

        public string H1
        {
            get { return h1; }
            set
            {
                h1 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string H2
        {
            get { return h2; }
            set
            {
                h2 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string H3
        {
            get { return h3; }
            set
            {
                h3 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string H4
        {
            get { return h4; }
            set
            {
                h4 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string H5
        {
            get { return h5; }
            set
            {
                h5 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string H6
        {
            get { return h6; }
            set
            {
                h6 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string H7
        {
            get { return h7; }
            set
            {
                h7 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string M1
        {
            get { return m1; }
            set
            {
                m1 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string M2
        {
            get { return m2; }
            set
            {
                m2 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string M3
        {
            get { return m3; }
            set
            {
                m3 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string M4
        {
            get { return m4; }
            set
            {
                m4 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string M5
        {
            get { return m5; }
            set
            {
                m5 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string M6
        {
            get { return m6; }
            set
            {
                m6 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public string M7
        {
            get { return m7; }
            set
            {
                m7 = value;
                CalculateTotal();
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CalculateTotal()
        {
            int h1Value, h2Value, h3Value, h4Value, h5Value, h6Value, h7Value;

            int m1Value, m2Value, m3Value, m4Value, m5Value, m6Value, m7Value;

            if (string.IsNullOrEmpty(H1)) h1Value = 0;
            else h1Value = Convert.ToInt32(H1);
            if (string.IsNullOrEmpty(H2)) h2Value = 0;
            else h2Value = Convert.ToInt32(H2);
            if(string.IsNullOrEmpty(H3)) h3Value = 0;
            else h3Value = Convert.ToInt32(H3);
            if(string.IsNullOrEmpty(H4)) h4Value = 0;
            else h4Value = Convert.ToInt32(H4);
            if(string.IsNullOrEmpty(H5)) h5Value = 0;
            else h5Value = Convert.ToInt32(H5);
            if(string.IsNullOrEmpty(H6)) h6Value = 0;
            else h6Value = Convert.ToInt32(H6);
            if(string.IsNullOrEmpty(H7)) h7Value = 0;
            else h7Value = Convert.ToInt32(H7);

            if (string.IsNullOrEmpty(M1)) m1Value = 0;
            else m1Value = Convert.ToInt32(M1);
            if (string.IsNullOrEmpty(M2)) m2Value = 0;
            else m2Value = Convert.ToInt32(M2);
            if (string.IsNullOrEmpty(M3)) m3Value = 0;
            else m3Value = Convert.ToInt32(M3);
            if (string.IsNullOrEmpty(M4)) m4Value = 0;
            else m4Value = Convert.ToInt32(M4);
            if (string.IsNullOrEmpty(M5)) m5Value = 0;
            else m5Value = Convert.ToInt32(M5);
            if (string.IsNullOrEmpty(M6)) m6Value = 0;
            else m6Value = Convert.ToInt32(M6);
            if (string.IsNullOrEmpty(M7)) m7Value = 0;
            else m7Value = Convert.ToInt32(M7);

            TotalHours = (h1Value + h2Value + h3Value + h4Value + h5Value + h6Value + h7Value).ToString();
            TotalMins = (m1Value + m2Value + m3Value + m4Value + m5Value + m6Value + m7Value).ToString();
        }
    }
}
