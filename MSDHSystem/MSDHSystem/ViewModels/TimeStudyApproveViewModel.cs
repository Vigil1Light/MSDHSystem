﻿using MSDHSystem.Models;
using MSDHSystem.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace MSDHSystem.ViewModels
{
    public class TimeStudyApproveViewModel : BaseViewModel, INotifyPropertyChanged
    {
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

        public ICommand ApproveCommand { get; set; }
        public ICommand RejectCommand { get; set; }
        public ICommand DetailCommand { get; set; }

        private ObservableCollection<TimeStudyApproveData> timeStudyApproveItems;
        public ObservableCollection<TimeStudyApproveData> TimeStudyApproveItems
        {
            get { return timeStudyApproveItems; }
            set
            {
                if (timeStudyApproveItems != value)
                {
                    timeStudyApproveItems = value;
                    OnPropertyChanged(nameof(timeStudyApproveItems));
                }
            }
        }

        public TimeStudyApproveViewModel()
        {
            TimeStudyApproveItems = new ObservableCollection<TimeStudyApproveData>();
            UpdateUI(false);
            Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
            newThread.Start();
            AppSessionManager.Instance.StartSession();
            ApproveCommand = new Command<TimeStudyApproveData>(OnApproveClicked);
            RejectCommand = new Command<TimeStudyApproveData>(OnRejectClicked);
            DetailCommand = new Command<TimeStudyApproveData>(OnDetailClicked);
        }

        private void OnDetailClicked(TimeStudyApproveData data)
        {
            DependencyService.Get<Toast>().Show(data.StartDate);
        }

        private void OnRejectClicked(TimeStudyApproveData data)
        {
            DependencyService.Get<Toast>().Show(data.EndDate);
        }

        private void OnApproveClicked(TimeStudyApproveData data)
        {
            DependencyService.Get<Toast>().Show(data.Name);
        }

        private void GetValue()
        {
            try
            {
                string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                string strQuery = string.Format("SELECT DISTINCT a.CalenderYear, a.CalenderWeek, b.Login_Name, b.pid_nmbr FROM TimeStudyDetail a INNER JOIN AD_Info b ON a.pid_nmbr = b.pid_nmbr WHERE SupervisorName = '{0}' AND SignedByEmployee = 'YES' AND (APPROVED IS NULL OR APPROVED <> 'Yes')", Xamarin.Essentials.SecureStorage.GetAsync("username").Result + "@msdh.ms.gov");
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
                        TimeStudyApproveItems.Add(new TimeStudyApproveData
                        {
                            No = n,
                            Name = reader["Login_Name"].ToString(),
                            StartDate = startDate.ToString("MM/dd/yyyy"),
                            EndDate = (startDate.AddDays(4).ToString("MM/dd/yyyy")),
                            BackColor = backColor
                        });
                        n++;
                    }
                }
                else
                {
                    DependencyService.Get<Toast>().Show("Can't find Time Study Approve data");
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
            /*if (firstWeek == 1)
            {
                
            }*/
            weekNum -= 1;
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
