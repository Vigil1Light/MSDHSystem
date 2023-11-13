using dotMorten.Xamarin.Forms;
using MSDHSystem.Models;
using MSDHSystem.Utils;
using MSDHSystem.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MSDHSystem.ViewModels
{
    public class TimeStudyDetailViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private bool isloading = false;
        private bool ischecked;
        private bool isenabled = true;
        private string name;
        private string email;
        private string jobname;
        private string location;
        private string orgcode;
        private string pin;
        private string pidnumber;
        private string startDate;
        private string endDate;

        private TimeStudyApproveData timeStudyApproveData = new TimeStudyApproveData();

        public bool IsLoading
        {
            get => isloading;
            set => SetProperty(ref isloading, value);
        }

        public bool IsChecked
        {
            get => ischecked;
            set => SetProperty(ref ischecked, value);
        }

        public bool IsEnabled
        {
            get => isenabled;
            set => SetProperty(ref isenabled, value);
        }

        public string Name
        {
            get { return name; }
            set => SetProperty(ref name, value);
        }
        public string Email
        {
            get { return email; }
            set => SetProperty(ref email, value);
        }
        public string Location
        {
            get { return location; }
            set => SetProperty(ref location, value);
        }
        public string Jobname
        {
            get { return jobname; }
            set => SetProperty(ref jobname, value);
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

        private string hours;
        public string Hours
        {
            get { return hours; }
            set
            {
                hours = value;
                OnPropertyChanged();
            }
        }

        private string mins;
        public string Mins
        {
            get { return mins; }
            set
            {
                mins = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TimeStudyDetailData> timeStudyDetailItems;
        public ObservableCollection<TimeStudyDetailData> TimeStudyDetailItems
        {
            get { return timeStudyDetailItems; }
            set
            {
                if (timeStudyDetailItems != value)
                {
                    timeStudyDetailItems = value;
                    OnPropertyChanged(nameof(TimeStudyDetailItems));
                }
            }
        }

        public TimeStudyDetailViewModel()
        {
            TimeStudyDetailItems = new ObservableCollection<TimeStudyDetailData>();
            if (Application.Current.Properties.ContainsKey("TimeStudyDetailValue"))
            {
                timeStudyApproveData = (TimeStudyApproveData)Application.Current.Properties["TimeStudyDetailValue"];
                GetValue(timeStudyApproveData);
                AppSessionManager.Instance.StartSession();
            }

        }

        private void CalculateSums()
        {
            int tHours = TimeStudyDetailItems.Sum(item => Convert.ToInt32(item.TotalHours));
            int tMins = TimeStudyDetailItems.Sum(item => Convert.ToInt32(item.TotalMins));
            Hours = (tHours + (tMins / 60)).ToString();
            Mins = (tMins % 60).ToString();
        }

        private void GetValue(TimeStudyApproveData timeStudyApproveDate)
        {
            try
            {
                string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                string strQuery = string.Format("SELECT * FROM AD_Info WHERE Login_Name = '{0}'", timeStudyApproveDate.Name);
                SqlConnection con = new SqlConnection(connstring);
                con.Open();
                SqlCommand command = new SqlCommand(strQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Name = timeStudyApproveDate.Name;
                        Email = reader["email"].ToString();
                        Location = reader["location"].ToString();
                        Jobname = reader["job_name"].ToString();
                        Orgcode = reader["org_code"].ToString();
                        Pin = reader["pin_win_nmbr"].ToString();
                        PIDnumber = reader["pid_nmbr"].ToString();
                        StartDate = timeStudyApproveDate.StartDate;
                        EndDate = timeStudyApproveDate.EndDate;
                    }
                }
                reader.Close();

                int n = 0;
                Color backColor = new Color();

                List<int> Date = timeStudyApproveDate.StartDate.ToString().Split('/').Select(int.Parse).ToList();
                DateTime startDate = new DateTime(Date[2], Date[0], Date[1]);
                strQuery = string.Format("SELECT * FROM TimeStudyDetail WHERE pid_nmbr = '{0}' AND CalenderYear = '{1}' AND CalenderWeek = '{2}'", PIDnumber, startDate.Year.ToString(), GetWeekNumber(startDate).ToString());
                command = new SqlCommand(strQuery, con);
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        n++;
                        if (n % 2 == 0)
                        {
                            backColor = Color.LightSkyBlue;
                        }
                        else
                        {
                            backColor = Color.LightCyan;
                        }
                        List<string> tHour = new List<string>();
                        List<string> tMin = new List<string>();
                        for (int i = 3; i < 9; i++)
                        {
                            if (reader[i] == null || reader[i].ToString() == "")
                            {
                                tHour.Add("0");
                                tMin.Add("0");
                            }
                            else
                            {
                                List<string> time = reader[i].ToString().Split(':').Select(item => string.IsNullOrEmpty(item) ? "0" : item).ToList();
                                tHour.Add(time[0]);
                                tMin.Add(time[1]);     
                            }
                        }
                        for (int i = 18; i < 20; i++)
                        {
                            if (reader[i] == null || reader[i].ToString() == "")
                            {
                                tHour.Add("0");
                                tMin.Add("0");
                            }
                            else
                            {
                                List<string> time = reader[i].ToString().Split(':').Select(item => string.IsNullOrEmpty(item) ? "0" : item).ToList();
                                tHour.Add(time[0]);
                                tMin.Add(time[1]);
                            }
                        }

                        TimeStudyDetailItems.Add(new TimeStudyDetailData
                        {
                            No = n,
                            Program = reader["Program"].ToString(),
                            Activity = reader["Activity"].ToString(),
                            H1 = tHour[0],
                            H2 = tHour[1],
                            H3 = tHour[2],
                            H4 = tHour[3],
                            H5 = tHour[4],
                            H6 = tHour[6],
                            H7 = tHour[7],
                            M1 = tMin[0],
                            M2 = tMin[1],
                            M3 = tMin[2],
                            M4 = tMin[3],
                            M5 = tMin[4],
                            M6 = tMin[6],
                            M7 = tMin[7],
                            TotalHours = tHour[5],
                            TotalMins = tHour[5],
                            BackColor = backColor
                        });
                        tHour.Clear();
                        tMin.Clear();
                        CalculateSums();
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

        public static int GetWeekNumber(DateTime thisDate)
        {
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(thisDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public void UpdateUI(bool state)
        {

            IsEnabled = state;
            IsLoading = !state;
        }

    }
}
