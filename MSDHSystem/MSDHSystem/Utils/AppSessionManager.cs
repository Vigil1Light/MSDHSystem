using MSDHSystem.Views;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace MSDHSystem.Utils
{
    public sealed class AppSessionManager
    {
        private static readonly Lazy<AppSessionManager> lazy = new Lazy<AppSessionManager>();
        public static AppSessionManager Instance { get { return lazy.Value; } }
        private Stopwatch StopWatch = new Stopwatch();
        private readonly int _sessionThreasholdMinutes = 40;
        public AppSessionManager()
        {
            SessionDuration = TimeSpan.FromMinutes(_sessionThreasholdMinutes);
        }
        private TimeSpan SessionDuration;
        public void EndSession()
        {
            if (StopWatch.IsRunning)
            {
                StopWatch.Stop();
            }
        }
        public void ExtendSession()
        {
            if (StopWatch.IsRunning)
            {
                StopWatch.Restart();
            }
        }
        public void StartSession()
        {
            if (!StopWatch.IsRunning)
            {
                StopWatch.Restart();
            }
            Console.WriteLine("Session Started at " + DateTime.Now.ToLongTimeString());
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                bool isTimerRunning = true;
                if (StopWatch.IsRunning && StopWatch.Elapsed.Minutes >= SessionDuration.Minutes) //User was inactive for N minutes
                {
                    RedirectAndInformInactivity();
                    EndSession();
                    isTimerRunning = false;
                }
                Console.WriteLine("Current Time Elapsed -" + StopWatch.Elapsed.ToString());
                return isTimerRunning;
            });
        }
        //TODO
        private void RedirectAndInformInactivity()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
                Application.Current.MainPage = new LoginPage();
            });
        }
    }
}
