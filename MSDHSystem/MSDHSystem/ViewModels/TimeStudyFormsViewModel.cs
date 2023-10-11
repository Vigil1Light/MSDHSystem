using MSDHSystem.Models;
using MSDHSystem.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace MSDHSystem.ViewModels
{
    public class TimeStudyFormsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private bool isloading = false;
        private bool ischecked;
        private bool isenabled = true;
        private string name;
        private string classification;
        private string orgcode;
        private string pin;
        private string pidnumber;
        private string startDate;
        private string endDate;
        private string supervisoremail;

        private TimeStudyDate timeStudyDate = new TimeStudyDate();
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
                timeStudyDate = (TimeStudyDate)Application.Current.Properties["TimeStudyDateValue"];
                GetValue(timeStudyDate);
                AppSessionManager.Instance.StartSession();
            }
            foreach (var item in TimeStudyItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalculateSums();
        }

        private void CalculateSums()
        {
            int tHours = TimeStudyItems.Sum(item => Convert.ToInt32(item.TotalHours));
            int tMins = TimeStudyItems.Sum(item => Convert.ToInt32(item.TotalMins));
            Hours = (tHours + (tMins / 60)).ToString();
            Mins = (tMins % 60).ToString();
        }

        private void OnSubmitForReviewClicked(object obj)
        {
            if (!ischecked || SupervisorEmail == "")
            {
                DependencyService.Get<Toast>().Show("Please sign and input supervisor email");
            }
        }

        private void OnSaveForLaterClicked(object obj)
        {
            int n = 0;
            for(int i = 0; i < TimeStudyItems.Count; i++)
            {
                if (timeStudyItems[i].Program == null) break;
                n++;
            }
            if(n == 0)
            {
                DependencyService.Get<Toast>().Show("Please input some data, can't save now");
            }
            else
            {

            }
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

                int n = 0;
                if (timeStudyDate.status == "(Saved for Later)")
                {
                    List<int> Date = timeStudyDate.startDate.ToString().Split('/').Select(int.Parse).ToList();
                    DateTime startDate = new DateTime(Date[2], Date[0], Date[1]);
                    strQuery = string.Format("SELECT * FROM TimeStudyDetail AS a INNER JOIN TimeStudyProgram AS b ON a.Program = b.ProgramCode INNER JOIN TimeStudyActivity AS c ON CAST(a.Activity AS INTEGER) = c.ActivityNumber WHERE pid_nmbr = '{0}' AND CalenderYear = '{1}' AND CalenderWeek = '{2}'", Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, startDate.Year.ToString(), GetWeekNumber(startDate).ToString());
                    command = new SqlCommand(strQuery, con);
                    reader = command.ExecuteReader();
                    
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            n++;
                            List<string> tHour = new List<string>();
                            List<string> tMin = new List<string>();
                            for (int i = 3; i < 8; i++)
                            {
                                if (reader[i] == null || reader[i].ToString() == "")
                                {
                                    tHour.Add("");
                                    tMin.Add("");
                                }
                                else
                                {
                                    List<int> time = reader[i].ToString().Split(':').Select(int.Parse).ToList();
                                    tHour.Add(time[0].ToString());
                                    tMin.Add(time[1].ToString());
                                }  
                            }
                            for (int i = 18; i < 20; i++)
                            {
                                if (reader[i] == null || reader[i].ToString() == "")
                                {
                                    tHour.Add("");
                                    tMin.Add("");
                                }
                                else
                                {
                                    List<int> time = reader[i].ToString().Split(':').Select(int.Parse).ToList();
                                    tHour.Add(time[0].ToString());
                                    tMin.Add(time[1].ToString());
                                }
                            }
                            TimeStudyItems.Add(new TimeStudyData
                            {
                                No = n,
                                Programs = programs,
                                Activities = activities,
                                Program = reader["ProgramCode"].ToString() + " " + reader["ProgramName"].ToString(),
                                Activity = reader["ActivityNumber"].ToString() + " " + reader["ActivityName"].ToString(),
                                H1 = tHour[0],
                                H2 = tHour[1],
                                H3 = tHour[2],
                                H4 = tHour[3],
                                H5 = tHour[4],
                                H6 = tHour[5],
                                H7 = tHour[6],
                                M1 = tMin[0],
                                M2 = tMin[1],
                                M3 = tMin[2],
                                M4 = tMin[3],
                                M5 = tMin[4],
                                M6 = tMin[5],
                                M7 = tMin[6],
                            });
                        }
                    }
                    reader.Close();
                }

                for (int i = n; i < 19; i++)
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

        public static int GetWeekNumber(DateTime thisDate)
        {
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(thisDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

    }
}
