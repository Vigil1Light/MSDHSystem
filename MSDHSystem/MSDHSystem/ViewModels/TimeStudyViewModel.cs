using MSDHSystem.Models;
using MSDHSystem.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Globalization;
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
                string strQuery = string.Format("SELECT * FROM MSDHFormsConfig WHERE Setting = 'TimeStudyMonth' AND Inactive = '0' AND (value4 = '{0}' OR value4 = '{1}')", DateTime.Now.Year.ToString(), (DateTime.Now.Year - 1).ToString());
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
                        int weeknumber = GetWeekNumber(dbDate);
                        string tmpStatus = "";
                        string tmpMonth = "";
                        bool tmpEnabled = true;
                        if (DateTime.Compare(dbDate, DateTime.Now) >= 0)
                        {
                            tmpStatus = "";
                            tmpMonth = "New Time Study";
                        }  
                        else
                        {
                            SqlConnection con1 = new SqlConnection(connstring);
                            con1.Open();
                            string query = string.Format("SELECT distinct * FROM  TimeStudyDetail a LEFT JOIN  TimeStudyEmmployeeInfo b ON a.pid_nmbr = b.PID_No WHERE a.pid_nmbr = '{0}' AND  FormType= 'TS' AND CalenderYear = '{1}' AND a.CalenderWeek = {2} ORDER BY a.CalenderWeek", Xamarin.Essentials.SecureStorage.GetAsync("pid_number").Result, dbDate.Year.ToString(), weeknumber.ToString());
                            SqlCommand command1 = new SqlCommand(query, con1);
                            SqlDataReader sqlDataReader = command1.ExecuteReader();
                            
                            
                            if (sqlDataReader.HasRows)
                            {
                                if (sqlDataReader.Read())
                                {
                                    if (sqlDataReader["APPROVED"].ToString() == "Yes")
                                    {
                                        continue;
                                    }
                                    if (sqlDataReader["APPROVED"] == null)
                                    {
                                        tmpStatus = "(Waiting Approve)";
                                        tmpEnabled = false;
                                    }
                                    else if (sqlDataReader["APPROVED"].ToString() != "Yes")
                                    {
                                        tmpStatus = "(Waiting Approve)";
                                        tmpEnabled = false;
                                    }
                                    if (sqlDataReader["SignedByEmployee"] == null)
                                    {
                                        tmpStatus = "(Saved for Later)";
                                        tmpEnabled = true;
                                    }
                                    else if (sqlDataReader["SignedByEmployee"].ToString() != "YES")
                                    {
                                        tmpStatus = "(Saved for Later)";
                                        tmpEnabled = true;
                                    }
                                }
                            }
                            else
                            {
                                tmpStatus = "Not Started";
                            }
                            sqlDataReader.Close();
                            con1.Close();
                            tmpMonth = reader["Value1"].ToString();
                        }
                        obMenus.Add(new TimeStudyDate
                        {
                            month = tmpMonth,
                            startDate = reader["Value2"].ToString(),
                            endDate = reader["Value3"].ToString(),
                            status = tmpStatus,     
                            IsEnabledCell = tmpEnabled
                        });
                        if (dbDate.Year >= DateTime.Now.Year && dbDate.Month > DateTime.Now.Month) break;
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

    }
}
