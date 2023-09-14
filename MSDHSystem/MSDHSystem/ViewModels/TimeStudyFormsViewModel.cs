using MSDHSystem.Models;
using MSDHSystem.Utils;
using MSDHSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace MSDHSystem.ViewModels
{
    public class TimeStudyFormsViewModel : BaseViewModel
    {
        private bool isloading = false;
        private string name;
        private string classification;
        private string orgcode;
        private string pin;
        private string pidnumber;
        private string startDate;
        private string endDate;

        public bool IsLoading
        {
            get => isloading;
            set => SetProperty(ref isloading, value);
        }

        public string Name
        {
            get { return name; }
            set => SetProperty(ref name, value);
        }

        public string Classification
        {
            get { return classification; }
            set => SetProperty(ref classification, value);
        }

        public string Orgcode
        {
            get { return orgcode; }
            set => SetProperty(ref orgcode, value);
        }

        public string Pin
        {
            get { return pin; }
            set => SetProperty(ref pin, value);
        }

        public string PIDnumber
        {
            get { return pidnumber; }
            set => SetProperty(ref pidnumber, value);
        }

        public string StartDate
        {
            get { return startDate; }
            set => SetProperty(ref startDate, value);
        }

        public string EndDate
        {
            get { return endDate; }
            set => SetProperty(ref endDate, value);
        }

        public TimeStudyFormsViewModel()
        {
            UpdateUI(false);
            if (Application.Current.Properties.ContainsKey("TimeStudyDateValue"))
            {
                TimeStudyDate timeStudyDate = (TimeStudyDate)Application.Current.Properties["TimeStudyDateValue"];
                Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
                newThread.Start(timeStudyDate);
                AppSessionManager.Instance.StartSession();
            }  
        }


        private void LongRunningTask(object parameter)
        {
            // Get the parameter value
            TimeStudyDate timeStudyDate = (TimeStudyDate)parameter;
            // Perform the long-running operation here
            // ...
            // Update UI from the background thread using Device.BeginInvokeOnMainThread
            Device.BeginInvokeOnMainThread(() =>
            {
                // Update UI to indicate that the operation has completed
                GetValue(timeStudyDate);
                UpdateUI(true);
            });
        }

        public void UpdateUI(bool state)
        {
            IsLoading = !state;
        }

        private void GetValue(TimeStudyDate timeStudyDate)
        {
            try
            {
                string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                string strQuery = string.Format("SELECT * FROM AD_Info WHERE Login_Name = '{0}'", Xamarin.Essentials.SecureStorage.GetAsync("username").Result);
                SqlConnection con = new SqlConnection(connstring);
                con.Open();
                SqlCommand command = new SqlCommand(strQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Name = reader["Login_Name"].ToString();
                        Classification = reader["location"].ToString();
                        Orgcode = reader["org_code"].ToString();
                        Pin = reader["pin_win_nmbr"].ToString();
                        PIDnumber = reader["pid_nmbr"].ToString();
                        StartDate = timeStudyDate.startDate;
                        EndDate = timeStudyDate.endDate;
                    }
                }
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                DependencyService.Get<Toast>().Show(ex.Message);
            }
        }

    }
}
