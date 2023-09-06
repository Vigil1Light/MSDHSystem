using MSDHSystem.Models;
using MSDHSystem.Utils;
using MSDHSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MSDHSystem.ViewModels
{
    public class SubMenuViewModel : BaseViewModel
    {
        ObservableCollection<Menus> obMenus;

        List<int> menuList;

        private bool isloading = false;
        private bool isenabled = true;
        private string headertext;


        public string HeaderText
        {
            get => headertext;
            set => SetProperty(ref headertext, value);
        }

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

        public SubMenuViewModel(string sm, ListView lstMenus)
        {
            HeaderText = sm;
            lstMenus_instance = lstMenus;
            UpdateUI(false);
            Thread newThread = new Thread(new ParameterizedThreadStart(LongRunningTask));
            newThread.Start(sm);
            AppSessionManager.Instance.StartSession();
        }

        private void AddMenus()
        {
            try
            {
                obMenus = new ObservableCollection<Menus>();
                for (int i = 0; i < menuList.Count; i++)
                {
                    string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                    string strQuery = string.Format("SELECT * FROM MSDHSitesDescription WHERE SiteID = '{0}'", menuList[i]);
                    SqlConnection con = new SqlConnection(connstring);
                    con.Open();
                    SqlCommand command = new SqlCommand(strQuery, con);
                    SqlDataReader reader = command.ExecuteReader();
                    string temp = "";
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            temp = reader.GetString(2);
                        }
                        obMenus.Add(new Menus
                        {
                            menuTitle = temp
                        });
                    }
                    reader.Close();
                    if (i == menuList.Count - 1)
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                DependencyService.Get<Toast>().Show(ex.Message);
            }
        }

        private void getMenu(string menu)
        {
            try
            {
                string connstring = @"data source=InventorySystem.mssql.somee.com;initial catalog=InventorySystem;user id=linglu626;password=linglu626;Connect Timeout=600";
                string strQuery = string.Format("SELECT * FROM MSDHMenuList WHERE MenuType = '{0}'", menu);
                SqlConnection con = new SqlConnection(connstring);
                con.Open();
                SqlCommand command = new SqlCommand(strQuery, con);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        string temp = reader.GetString(2);
                        menuList = temp.Split(',').Select(int.Parse).ToList();
                    }
                }
                reader.Close();
                con.Close();
            }
            catch(Exception ex)
            {
                DependencyService.Get<Toast>().Show(ex.Message);
            }
        }

        private void LongRunningTask(object parameter)
        {
            // Get the parameter value
            string parameterValue = parameter as string;

            // Perform the long-running operation here
            // ...
            getMenu(parameterValue);
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
