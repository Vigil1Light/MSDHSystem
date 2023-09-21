using MSDHSystem.Models;
using MSDHSystem.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
        private string supervisoremail;

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

        public string SupervisorEmail
        {
            get { return supervisoremail; }
            set => SetProperty(ref supervisoremail, value);
        }

        public List<string> programs = new List<string>();

        public List<string> activities = new List<string>();

        public Command SaveForLater_Clicked { get; }
        public Command SubmitForReview_Clicked { get; }

        private ObservableCollection<TimeStudyData> timeStudyItems;
        public ObservableCollection<TimeStudyData> TimeStudyItems
        {
            get { return timeStudyItems; }
            set
            {
                if (timeStudyItems != value)
                {
                    timeStudyItems = value;
                    OnPropertyChanged(nameof(TimeStudyItems));
                }
            }
        }

        public TimeStudyFormsViewModel()
        {
            SaveForLater_Clicked = new Command(OnSaveForLaterClicked);
            SubmitForReview_Clicked = new Command(OnSubmitForReviewClicked);
            TimeStudyItems = new ObservableCollection<TimeStudyData>();
            if (Application.Current.Properties.ContainsKey("TimeStudyDateValue"))
            {
                TimeStudyDate timeStudyDate = (TimeStudyDate)Application.Current.Properties["TimeStudyDateValue"];
                GetValue(timeStudyDate);
                AppSessionManager.Instance.StartSession();
            }  
        }

        private void OnSubmitForReviewClicked(object obj)
        {
            
        }

        private void OnSaveForLaterClicked(object obj)
        {
            DependencyService.Get<Toast>().Show(TimeStudyItems[0].M1.ToString());
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
                        Classification = reader["location"].ToString() + "/" + reader["job_name"].ToString();
                        Orgcode = reader["org_code"].ToString();
                        Pin = reader["pin_win_nmbr"].ToString();
                        PIDnumber = reader["pid_nmbr"].ToString();
                        StartDate = timeStudyDate.startDate;
                        EndDate = timeStudyDate.endDate;
                    }
                }
                reader.Close();

                strQuery = "SELECT a.ProgramTitleID, b.ProgramName as ProgramTitle, a.ProgramCode, a.ProgramName FROM TimeStudyProgram AS a INNER JOIN TimeStudyProgramTitle AS b ON a.ProgramTitleID = b.Seq ORDER BY a.ProgramTitleID ASC";
                command = new SqlCommand(strQuery, con);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    string stemp = "";
                    while (reader.Read())
                    {
                        if(stemp != reader["ProgramTitle"].ToString())
                        {
                            programs.Add("------" + reader["ProgramTitle"].ToString() + "------");
                        }
                        programs.Add(reader["ProgramCode"].ToString() + " " + reader["ProgramName"].ToString());
                        stemp = reader["ProgramTitle"].ToString();
                    }
                }
                reader.Close();

                strQuery = "SELECT * FROM TimeStudyActivity";
                command = new SqlCommand(strQuery, con);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        activities.Add(reader["ActivityNumber"].ToString() + " " + reader["ActivityName"].ToString());
                    }
                }
                reader.Close();

                for(int i = 0; i < 19; i++)
                {
                    TimeStudyItems.Add(new TimeStudyData
                    {
                        No = i + 1,
                        Programs = programs,
                        Activities = activities,
                });
                }

                con.Close();

            }
            catch (Exception ex)
            {
                DependencyService.Get<Toast>().Show(ex.Message);
            }
        }

    }
}
