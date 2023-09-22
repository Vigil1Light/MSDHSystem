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


        public TimeStudyViewModel(ListView lstMenus)
        {
            AddMenus();
            lstMenus.ItemsSource = obMenus;
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
                        List<int> Date = reader["Value2"].ToString().Split('/').Select(int.Parse).ToList();
                        DateTime dbDate = new DateTime(Date[2], Date[0], Date[1]);
                        string tmpStatus = "";
                        if (DateTime.Compare(dbDate, DateTime.Now) >= 0)
                            tmpStatus = "(New Time Study)";
                        else if ((bool)reader["Inactive"] == true)
                            tmpStatus = "(Waiting Approve)";
                        else tmpStatus = "(Not Started)";
                        obMenus.Add(new TimeStudyDate
                        {
                            month = reader["Value1"].ToString(),
                            startDate = reader["Value2"].ToString(),
                            endDate = reader["Value3"].ToString(),
                            status = tmpStatus,      
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

    }
}
