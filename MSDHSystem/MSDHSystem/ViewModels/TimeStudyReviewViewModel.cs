using MSDHSystem.Models;
using MSDHSystem.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using Xamarin.Forms;

namespace MSDHSystem.ViewModels
{
    public class TimeStudyReviewViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private bool isloading = false;
        private bool isenabled = true;
        private string name;
        private string jobname;
        private string location;
        private string orgcode;
        private string pin;
        private string pidnumber;

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

        public string Name
        {
            get { return name; }
            set => SetProperty(ref name, value);
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


        private ObservableCollection<TimeStudyReviewData> timeStudyRetivewItems;
        public ObservableCollection<TimeStudyReviewData> TimeStudyReviewItems
        {
            get { return timeStudyRetivewItems; }
            set
            {
                if (timeStudyRetivewItems != value)
                {
                    timeStudyRetivewItems = value;
                    OnPropertyChanged(nameof(timeStudyRetivewItems));
                }
            }
        }

        public TimeStudyReviewViewModel()
        {
            TimeStudyReviewItems = new ObservableCollection<TimeStudyReviewData>();
            UpdateUI(false);
            Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
            newThread.Start();
            AppSessionManager.Instance.StartSession();
        }

        private void GetValue()
        {
            try
            {
                string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                string strQuery = string.Format("SELECT * FROM  TimeStudyDetail a INNER JOIN AD_Info b ON a.pid_nmbr = b.pid_nmbr WHERE a.PIN = '{0}' ORDER BY CAST(a.CalenderYear AS INT) ASC, CAST(a.CalenderWeek AS INT) ASC", Xamarin.Essentials.SecureStorage.GetAsync("pin_number").Result);
                SqlConnection con = new SqlConnection(connstring);
                con.Open();
                SqlCommand command = new SqlCommand(strQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                int n = 1;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Color backColor = new Color();
                        if (n % 2 == 0)
                        {
                            backColor = Color.LightSkyBlue;
                        }
                        else
                        {
                            backColor = Color.LightCyan;
                        }
                        DateTime startDate = GetDate(int.Parse(reader["CalenderYear"].ToString()), int.Parse(reader["CalenderWeek"].ToString()));
                        TimeStudyReviewItems.Add(new TimeStudyReviewData
                        {
                            No = n,
                            StartDate = startDate.ToString("MM/dd/yyyy"),
                            EndDate = (startDate.AddDays(4).ToString("MM/dd/yyyy")),
                            Program = reader["Program"].ToString(),
                            Activity = reader["Activity"].ToString(),
                            T1 = reader["MondayTime"].ToString(),
                            T2 = reader["TuesdayTime"].ToString(),
                            T3 = reader["WednesdayTime"].ToString(),
                            T4 = reader["ThursdayTime"].ToString(),
                            T5 = reader["FridayTime"].ToString(),
                            T6 = reader["SaturdayTime"].ToString(),
                            T7 = reader["SundayTime"].ToString(),
                            Total = reader["TotalTime"].ToString(),
                            BackColor = backColor
                        });
                        n++;
                        if (n == 2)
                        {
                            Name = reader["Login_Name"].ToString();
                            Location = reader["location"].ToString();
                            Jobname = reader["job_name"].ToString();
                            Orgcode = reader["org_code"].ToString();
                            Pin = reader["pin_win_nmbr"].ToString();
                            PIDnumber = reader["pid_nmbr"].ToString();
                        }
                    }
                }
                else
                {
                    DependencyService.Get<Toast>().Show("Can't find Time Study Review data");
                }
                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                DependencyService.Get<Toast>().Show(ex.Message);
            }
        }

        public static DateTime GetDate(int year, int weeknumber)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weeknumber;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }

        public void UpdateUI(bool state)
        {

            IsEnabled = state;
            IsLoading = !state;
        }

        private void LongRunningTask(object parameter)
        {
            // Get the parameter value
            string parameterValue = parameter as string;

            // Perform the long-running operation here
            // ...
            GetValue();
            // Update UI from the background thread using Device.BeginInvokeOnMainThread
            Device.BeginInvokeOnMainThread(() =>
            {
                // Update UI to indicate that the operation has completed 
                UpdateUI(true);
            });
        }
    }
}
