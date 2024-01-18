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
    public class LabTimeStudyFormsViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private bool isloading = false;
        private bool ischecked;
        private bool isenabled = true;
        private string name;
        private string jobname;
        private string location;
        private string orgcode;
        private string pin;
        private string pidnumber;
        private string startDate;
        private string endDate;

        private AutoSuggestBox suggestBox;

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

        public LabTimeStudyFormsViewModel(AutoSuggestBox autoSuggestBox)
        {
            suggestBox = autoSuggestBox;
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
            if (!ischecked || suggestBox.Text == "")
            {
                DependencyService.Get<Toast>().Show("Please sign and input supervisor email");
            }
            else
            {
                if (Xamarin.Essentials.SecureStorage.GetAsync("submitstate").Result == "1")
                {
                    UpdateUI(false);
                    Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
                    newThread.Start("submit");
                }
                else
                {
                    DependencyService.Get<Toast>().Show("Please select correct supervisor email");
                }
            }
        }

        private void OnSaveForLaterClicked(object obj)
        {
            if (IsChecked)
            {
                DependencyService.Get<Toast>().Show("Can't save for later with sign employee, please remove checkbox");
            }
            else
            {
                UpdateUI(false);
                Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
                newThread.Start("save");
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
                string connstring = @"data source=155.254.244.41;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
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
                        Location = reader["location"].ToString();
                        Jobname = reader["job_name"].ToString();
                        Orgcode = reader["org_code"].ToString();
                        Pin = reader["pin_win_nmbr"].ToString();
                        PIDnumber = reader["pid_nmbr"].ToString();
                        StartDate = timeStudyDate.startDate;
                        EndDate = timeStudyDate.endDate;
                    }
                }
                reader.Close();

                strQuery = "SELECT * FROM TimeStudyLabProgram";
                command = new SqlCommand(strQuery, con);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        programs.Add(reader["RecordID"].ToString() + " " + reader["Labortory"].ToString());
                    }
                }
                reader.Close();

                strQuery = "SELECT * FROM TimeStudyLabActivity";
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
                Color backColor = new Color();
                if (timeStudyDate.status == "(Saved for Later)")
                {
                    List<int> Date = timeStudyDate.startDate.ToString().Split('/').Select(int.Parse).ToList();
                    DateTime startDate = new DateTime(Date[2], Date[0], Date[1]);
                    strQuery = string.Format("SELECT * FROM TimeStudyDetail AS a INNER JOIN TimeStudyLabProgram AS b ON a.Program = b.Labortory INNER JOIN TimeStudyLabActivity AS c ON CAST(a.Activity AS INTEGER) = c.RecordID WHERE pid_nmbr = '{0}' AND CalenderYear = '{1}' AND CalenderWeek = '{2}'", Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, startDate.Year.ToString(), GetWeekNumber(startDate).ToString());
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
                                Program = reader["RecordID"].ToString() + " " + reader["Labortory"].ToString(),
                                Activity = reader["Activity"].ToString() + " " + reader["ActivityName"].ToString(),
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
                                BackColor = backColor
                            });
                            tHour.Clear();
                            tMin.Clear();
                        }
                    }
                    reader.Close();
                }

                for (int i = n; i < 19; i++)
                {
                    if (i % 2 == 0)
                    {
                        backColor = Color.LightCyan;
                    }
                    else
                    {
                        backColor = Color.LightSkyBlue;
                    }
                    TimeStudyItems.Add(new TimeStudyData
                    {
                        No = i + 1,
                        Programs = programs,
                        Activities = activities,
                        BackColor = backColor
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

        public void UpdateUI(bool state)
        {

            IsEnabled = state;
            IsLoading = !state;
        }

        public void Submit()
        {
            /*            for(int i = 0; i < TimeStudyItems.Count; i++)
                        {
                            if (TimeStudyItems[i].TotalHours != null && int.Parse(TimeStudyItems[i].TotalHours) < 35)
                            {
                                DependencyService.Get<Toast>().Show("The total hours of every week must be greater than 35 hours");
                                return;
                            }
                        }*/

            List<int> Date = timeStudyDate.startDate.ToString().Split('/').Select(int.Parse).ToList();
            DateTime startDate = new DateTime(Date[2], Date[0], Date[1]);

            string connstring = @"data source=155.254.244.41;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
            SqlConnection con = new SqlConnection(connstring);
            con.Open();

            int n = 0;
            for (int i = 0; i < TimeStudyItems.Count; i++)
            {
                if (TimeStudyItems[i].Program == null)
                {
                    continue;
                }
                else
                {
                    List<string> tempHour = new List<string>();
                    List<string> tempMin = new List<string>();
                    List<string> tempTime = new List<string>();

                    if (TimeStudyItems[i].H1 == null || TimeStudyItems[i].H1 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H1);
                    }
                    if (TimeStudyItems[i].H2 == null || TimeStudyItems[i].H2 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H2);
                    }
                    if (TimeStudyItems[i].H3 == null || TimeStudyItems[i].H3 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H3);
                    }
                    if (TimeStudyItems[i].H4 == null || TimeStudyItems[i].H4 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H4);
                    }
                    if (TimeStudyItems[i].H5 == null || TimeStudyItems[i].H5 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H5);
                    }
                    if (TimeStudyItems[i].H6 == null || TimeStudyItems[i].H6 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H6);
                    }
                    if (TimeStudyItems[i].H7 == null || TimeStudyItems[i].H7 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H7);
                    }

                    if (TimeStudyItems[i].M1 == null || TimeStudyItems[i].M1 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M1);
                    }
                    if (TimeStudyItems[i].M2 == null || TimeStudyItems[i].M2 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M2);
                    }
                    if (TimeStudyItems[i].M3 == null || TimeStudyItems[i].M3 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M3);
                    }
                    if (TimeStudyItems[i].M4 == null || TimeStudyItems[i].M4 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M4);
                    }
                    if (TimeStudyItems[i].M5 == null || TimeStudyItems[i].M5 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M5);
                    }
                    if (TimeStudyItems[i].M6 == null || TimeStudyItems[i].M6 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M6);
                    }
                    if (TimeStudyItems[i].M7 == null || TimeStudyItems[i].M7 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M7);
                    }

                    for (int j = 0; j < tempHour.Count; j++)
                    {
                        if (tempHour[j] == "0" && tempMin[j] == "0")
                        {
                            tempTime.Add("");
                        }
                        else
                        {
                            tempTime.Add(tempHour[j] + ":" + tempMin[j]);
                        }
                    }

                    string strQuery = string.Format("DELETE FROM TimeStudyDetail WHERE pid_nmbr = '{0}' AND Program = '{1}' AND Activity = '{2}' AND CalenderYear = '{3}' AND CalenderWeek = '{4}'", Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, TimeStudyItems[i].Program.Split(' ')[0], TimeStudyItems[i].Activity.Split(' ')[0], startDate.Year.ToString(), GetWeekNumber(startDate).ToString());

                    SqlCommand command = new SqlCommand(strQuery, con);
                    command.ExecuteNonQuery();

                    strQuery = string.Format("INSERT INTO TimeStudyDetail(PIN, Program, Activity, Mondaytime, TuesdayTime, WednesdayTime, ThursdayTime, FridayTime, SaturdayTime, SundayTime, TotalTime, SupervisorName, DateCompleted, CalenderWeek, pid_nmbr, CalenderYear, SignedByEmployee) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', 'YES')", Xamarin.Essentials.SecureStorage.GetAsync("pin_number").Result, TimeStudyItems[i].Program.Split(' ')[0], TimeStudyItems[i].Activity.Split(' ')[0], tempTime[0], tempTime[1], tempTime[2], tempTime[3], tempTime[4], tempTime[5], tempTime[6], TimeStudyItems[i].TotalHours + ":" + TimeStudyItems[i].TotalMins, suggestBox.Text, DateTime.Today.ToString("MM/dd/yyyy"), GetWeekNumber(startDate).ToString(), Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, startDate.Year.ToString());
                    command = new SqlCommand(strQuery, con);
                    command.ExecuteNonQuery();

                    /*strQuery = string.Format("DELETE FROM TimeStudyEmmployeeInfo WHERE PIN = '{0}' AND CalenderWeek = '{1}' AND FormType = 'TS'",Xamarin.Essentials.SecureStorage.GetAsync("pin_number").Result, GetWeekNumber(startDate).ToString());
                    command = new SqlCommand(strQuery, con);
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                        }
                    }*/

                    strQuery = string.Format("INSERT INTO TimeStudyEmmployeeInfo(EmployeeName, PIN, Classification, ORG, PID_NO, CalenderWeek, FormType) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', 'TS')", Xamarin.Essentials.SecureStorage.GetAsync("username").Result, Xamarin.Essentials.SecureStorage.GetAsync("pin_number").Result, Location + "/" + Jobname, Orgcode, Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, GetWeekNumber(startDate).ToString());
                    command = new SqlCommand(strQuery, con);
                    command.ExecuteNonQuery();

                    tempHour.Clear();
                    tempMin.Clear();
                    tempTime.Clear();
                    n++;
                }
            }
            if (n == 0)
            {
                DependencyService.Get<Toast>().Show("Please input some data, can't submit now");
            }
            else
            {
                DependencyService.Get<Toast>().Show("Successfully submitted");
            }
            con.Close();

            SendMail("TS", Xamarin.Essentials.SecureStorage.GetAsync("username").Result);
        }

        public void Save()
        {
            /*            for (int i = 0; i < TimeStudyItems.Count; i++)
                        {
                            if (TimeStudyItems[i].TotalHours != null && int.Parse(TimeStudyItems[i].TotalHours) < 35)
                            {
                                DependencyService.Get<Toast>().Show("The total hours of every week must be greater than 35 hours");
                                return;
                            }
                        }*/

            List<int> Date = timeStudyDate.startDate.ToString().Split('/').Select(int.Parse).ToList();
            DateTime startDate = new DateTime(Date[2], Date[0], Date[1]);

            string connstring = @"data source=155.254.244.41;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
            SqlConnection con = new SqlConnection(connstring);
            con.Open();

            int n = 0;
            for (int i = 0; i < TimeStudyItems.Count; i++)
            {
                if (TimeStudyItems[i].Program == null)
                {
                    continue;
                }
                else
                {
                    List<string> tempHour = new List<string>();
                    List<string> tempMin = new List<string>();
                    List<string> tempTime = new List<string>();

                    if (TimeStudyItems[i].H1 == null || TimeStudyItems[i].H1 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H1);
                    }
                    if (TimeStudyItems[i].H2 == null || TimeStudyItems[i].H2 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H2);
                    }
                    if (TimeStudyItems[i].H3 == null || TimeStudyItems[i].H3 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H3);
                    }
                    if (TimeStudyItems[i].H4 == null || TimeStudyItems[i].H4 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H4);
                    }
                    if (TimeStudyItems[i].H5 == null || TimeStudyItems[i].H5 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H5);
                    }
                    if (TimeStudyItems[i].H6 == null || TimeStudyItems[i].H6 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H6);
                    }
                    if (TimeStudyItems[i].H7 == null || TimeStudyItems[i].H7 == "")
                    {
                        tempHour.Add("0");
                    }
                    else
                    {
                        tempHour.Add(TimeStudyItems[i].H7);
                    }

                    if (TimeStudyItems[i].M1 == null || TimeStudyItems[i].M1 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M1);
                    }
                    if (TimeStudyItems[i].M2 == null || TimeStudyItems[i].M2 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M2);
                    }
                    if (TimeStudyItems[i].M3 == null || TimeStudyItems[i].M3 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M3);
                    }
                    if (TimeStudyItems[i].M4 == null || TimeStudyItems[i].M4 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M4);
                    }
                    if (TimeStudyItems[i].M5 == null || TimeStudyItems[i].M5 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M5);
                    }
                    if (TimeStudyItems[i].M6 == null || TimeStudyItems[i].M6 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M6);
                    }
                    if (TimeStudyItems[i].M7 == null || TimeStudyItems[i].M7 == "")
                    {
                        tempMin.Add("0");
                    }
                    else
                    {
                        tempMin.Add(TimeStudyItems[i].M7);
                    }

                    for (int j = 0; j < tempHour.Count; j++)
                    {
                        if (tempHour[j] == "0" && tempMin[j] == "0")
                        {
                            tempTime.Add("");
                        }
                        else
                        {
                            tempTime.Add(tempHour[j] + ":" + tempMin[j]);
                        }
                    }

                    string strQuery = string.Format("DELETE FROM TimeStudyDetail WHERE pid_nmbr = '{0}' AND Program = '{1}' AND Activity = '{2}' AND CalenderYear = '{3}' AND CalenderWeek = '{4}'", Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, TimeStudyItems[i].Program.Split(' ')[0], TimeStudyItems[i].Activity.Split(' ')[0], startDate.Year.ToString(), GetWeekNumber(startDate).ToString());

                    SqlCommand command = new SqlCommand(strQuery, con);
                    command.ExecuteNonQuery();

                    strQuery = string.Format("INSERT INTO TimeStudyDetail(PIN, Program, Activity, Mondaytime, TuesdayTime, WednesdayTime, ThursdayTime, FridayTime, SaturdayTime, SundayTime, TotalTime, SupervisorName, DateCompleted, CalenderWeek, pid_nmbr, CalenderYear) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}')", Xamarin.Essentials.SecureStorage.GetAsync("pin_number").Result, TimeStudyItems[i].Program.Split(' ')[0], TimeStudyItems[i].Activity.Split(' ')[0], tempTime[0], tempTime[1], tempTime[2], tempTime[3], tempTime[4], tempTime[5], tempTime[6], TimeStudyItems[i].TotalHours + ":" + TimeStudyItems[i].TotalMins, suggestBox.Text, DateTime.Today.ToString("MM/dd/yyyy"), GetWeekNumber(startDate).ToString(), Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, startDate.Year.ToString());
                    command = new SqlCommand(strQuery, con);
                    command.ExecuteNonQuery();

                    /*strQuery = string.Format("DELETE FROM TimeStudyEmmployeeInfo WHERE PIN = '{0}' AND CalenderWeek = '{1}' AND FormType = 'TS'",Xamarin.Essentials.SecureStorage.GetAsync("pin_number").Result, GetWeekNumber(startDate).ToString());
                    command = new SqlCommand(strQuery, con);
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                        }
                    }*/

                    strQuery = string.Format("INSERT INTO TimeStudyEmmployeeInfo(EmployeeName, PIN, Classification, ORG, PID_NO, CalenderWeek, FormType) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', 'TS')", Xamarin.Essentials.SecureStorage.GetAsync("username").Result, Xamarin.Essentials.SecureStorage.GetAsync("pin_number").Result, Location + "/" + Jobname, Orgcode, Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, GetWeekNumber(startDate).ToString());
                    command = new SqlCommand(strQuery, con);
                    command.ExecuteNonQuery();

                    tempHour.Clear();
                    tempMin.Clear();
                    tempTime.Clear();
                    n++;
                }
            }
            if (n == 0)
            {
                DependencyService.Get<Toast>().Show("Please input some data, can't save now");
            }
            else
            {
                DependencyService.Get<Toast>().Show("Successfully saved");
            }
            con.Close();
        }
        private void LongRunningTask(object parameter)
        {
            // Get the parameter value
            string parameterValue = parameter as string;

            // Perform the long-running operation here
            // ...

            // Update UI from the background thread using Device.BeginInvokeOnMainThread
            Device.BeginInvokeOnMainThread(() =>
            {
                // Update UI to indicate that the operation has completed
                if (parameterValue == "submit") Submit();
                else if (parameterValue == "save") Save();
                //await Task.Delay(1000);
                //await Shell.Current.GoToAsync(nameof(TimeStudyPage));
                UpdateUI(true);
            });
        }

        public void SendMail(string formid, string recordid)
        {
            string connectionString = @"data source=155.254.244.41;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
            string storedProcedureName = "sp_FormSubmissionNotification";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add any input parameters if required
                        command.Parameters.AddWithValue("@formid", formid);
                        command.Parameters.AddWithValue("@recordid", recordid);

                        // Add any output parameters if required
                        //command.Parameters.Add("@OutputParameterName", SqlDbType.Int, 0).Direction = ParameterDirection.Output;

                        connection.Open();

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        // Access the output parameters if required
                        //string outputValue = command.Parameters["@OutputParameterName"].Value.ToString();

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<Toast>().Show(ex.Message);
            }

        }

    }
}
