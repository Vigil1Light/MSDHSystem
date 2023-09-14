using MSDHSystem.Models;
using MSDHSystem.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace MSDHSystem.ViewModels
{
    public class TimeStudyViewModel : BaseViewModel
    {
        ObservableCollection<TimeStudyDate> obMenus;

        private bool isloading = false;
        private bool isenabled = true;

        public bool IsLoading
        {
            get => isloading;
            set => SetProperty(ref isloading, value);
        }

        public bool IsEnabled
        {
            get => isenabled;
            set => SetProperty(ref isenabled, value);
        }

        public ListView lstMenus_instance;

        public TimeStudyViewModel(ListView lstMenus)
        {
            lstMenus_instance = lstMenus;
            UpdateUI(false);
            AddMenus();
            Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
            newThread.Start();
            AppSessionManager.Instance.StartSession();
        }

        private void AddMenus()
        {
            try
            {
                obMenus = new ObservableCollection<TimeStudyDate>();
                string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                string strQuery = string.Format("SELECT * FROM MSDHFormsConfig WHERE Setting = 'TimeStudyMonth'");
                SqlConnection con = new SqlConnection(connstring);
                con.Open();
                SqlCommand command = new SqlCommand(strQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        obMenus.Add(new TimeStudyDate
                        {
                            month = reader.GetString(2),
                            startDate = reader.GetString(3),
                            endDate = reader.GetString(4),
                        });
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

        private void LongRunningTask(object parameter)
        {
            // Get the parameter value

            // Perform the long-running operation here
            // ...
            AddMenus();
            // Update UI from the background thread using Device.BeginInvokeOnMainThread
            Device.BeginInvokeOnMainThread(() =>
            {
                // Update UI to indicate that the operation has completed
                lstMenus_instance.ItemsSource = obMenus;
                UpdateUI(true);
            });
        }

        public void UpdateUI(bool state)
        {

            IsEnabled = state;
            IsLoading = !state;
        }
    }
}
