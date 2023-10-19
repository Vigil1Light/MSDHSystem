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
        public List<string> Programs { get; set; }
        public List<string> Activities { get; set; }
        public Color BackColor { get; set; }

        private string h1;
        private string h2;
        private string h3;
        private string h4;
        private string h5;
        private string h6;
        private string h7;
        private string m1;
        private string m2;
        private string m3;
        private string m4;
        private string m5;
        private string m6;
        private string m7;
        private string totalHours { get; set; }
        private string totalMins { get; set; }
        private string program { get; set; }
        private string activity { get; set; }
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
        public string Program
        {
            get { return program; }
            set
            {
                program = value;
                OnPropertyChanged();
            }
        }
        public string Activity
        {
            get { return activity; }
            set
            {
                activity = value;
                OnPropertyChanged();
            }
        }

        public string H1
        {
            get { return h1; }
            set
            {
                if (CheckRole())
                {
                    h1 = value;
                    CalculateTotal();

                }
                else h1 = null;
                OnPropertyChanged();
            }
        }

        public string H2
        {
            get { return h2; }
            set
            {
                if (CheckRole())
                {
                    h2 = value;
                    CalculateTotal();

                }
                else h2 = null;
                OnPropertyChanged();
            }
        }

        public string H3
        {
            get { return h3; }
            set
            {
                if (CheckRole())
                {
                    h3 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else h3 = null;
                OnPropertyChanged();
            }
        }

        public string H4
        {
            get { return h4; }
            set
            {
                if (CheckRole())
                {
                    h4 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else h4 = null;
                OnPropertyChanged();
            }
        }

        public string H5
        {
            get { return h5; }
            set
            {
                if (CheckRole())
                {
                    h5 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else h5 = null;
                OnPropertyChanged();
            }
        }

        public string H6
        {
            get { return h6; }
            set
            {
                if (CheckRole())
                {
                    h6 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else h6 = null;
                OnPropertyChanged();
            }
        }

        public string H7
        {
            get { return h7; }
            set
            {
                if (CheckRole())
                {
                    h7 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else h7 = null;
                OnPropertyChanged();
            }
        }

        public string M1
        {
            get { return m1; }
            set
            {
                if (CheckRole())
                {
                    m1 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else m1 = null;
                OnPropertyChanged();
            }
        }

        public string M2
        {
            get { return m2; }
            set
            {
                if (CheckRole())
                {
                    m2 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else m2 = null;
                OnPropertyChanged();
            }
        }

        public string M3
        {
            get { return m3; }
            set
            {
                if (CheckRole())
                {
                    m3 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else m3 = null;
                OnPropertyChanged();
            }
        }

        public string M4
        {
            get { return m4; }
            set
            {
                if (CheckRole())
                {
                    m4 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else m4 = null;
                OnPropertyChanged();
            }
        }

        public string M5
        {
            get { return m5; }
            set
            {
                if (CheckRole())
                {
                    m5 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else m5 = null;
                OnPropertyChanged();
            }
        }

        public string M6
        {
            get { return m6; }
            set
            {
                if (CheckRole())
                {
                    m6 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else m6 = null;
                OnPropertyChanged();
            }
        }

        public string M7
        {
            get { return m7; }
            set
            {
                if (CheckRole())
                {
                    m7 = value;
                    CalculateTotal();
                    OnPropertyChanged();
                }
                else m7 = null;
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

            int tHours, tMinutes;

            if (string.IsNullOrEmpty(H1)) h1Value = 0;
            else h1Value = Convert.ToInt32(H1);
            if (string.IsNullOrEmpty(H2)) h2Value = 0;
            else h2Value = Convert.ToInt32(H2);
            if (string.IsNullOrEmpty(H3)) h3Value = 0;
            else h3Value = Convert.ToInt32(H3);
            if (string.IsNullOrEmpty(H4)) h4Value = 0;
            else h4Value = Convert.ToInt32(H4);
            if (string.IsNullOrEmpty(H5)) h5Value = 0;
            else h5Value = Convert.ToInt32(H5);
            if (string.IsNullOrEmpty(H6)) h6Value = 0;
            else h6Value = Convert.ToInt32(H6);
            if (string.IsNullOrEmpty(H7)) h7Value = 0;
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

            tHours = (m1Value + m2Value + m3Value + m4Value + m5Value + m6Value + m7Value) / 60;
            tMinutes = (m1Value + m2Value + m3Value + m4Value + m5Value + m6Value + m7Value) % 60;
            TotalHours = (h1Value + h2Value + h3Value + h4Value + h5Value + h6Value + h7Value + tHours).ToString();
            TotalMins = tMinutes.ToString();
        }

        private bool CheckRole()
        {
            if (Program == null || Activity == null)
            {
                DependencyService.Get<Toast>().Show("Please select Program(" + No.ToString() + ")" + " and Activity(" + No.ToString() + ")" + " first");
                return false;
            }
            else return true;
        }
    }
}
