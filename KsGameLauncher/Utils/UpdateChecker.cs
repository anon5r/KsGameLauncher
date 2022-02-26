using System;
using System.Diagnostics;
using System.Windows.Threading;
using KsGameLauncher.Structures;

namespace KsGameLauncher.Utils
{
    internal class UpdateChecker
    {
        private DateTime lastChecked;
        private DispatcherTimer timer;

        private static UpdateChecker instance;

        private UpdateChecker()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(CheckUpdate);
            timer.Start();
        }

        private static void CreateTimer()
        {
            if (instance == null)
            {
                instance = new UpdateChecker();
            }
        }

        public static void CreateUpdateCheker()
        {
            CreateUpdateCheker(0, CheckInterval.UnitType.Minutes);
        }

        public static void CreateUpdateCheker(int interval)
        {
            CheckInterval.UnitType unit = Properties.Settings.Default.UpdateCheckIntervalUnit;
            CreateUpdateCheker(interval, unit);
        }

        public static void CreateUpdateCheker(int interval, CheckInterval.UnitType unit)
        {
            CreateTimer();

            Debug.WriteLine(String.Format("[CreateUpdateCheker {0}] Start check update", DateTime.Now));
            
            if (interval == 0)
            {
                instance.timer.Stop();
            }
            else
            {
                switch (unit)
                {
                    case CheckInterval.UnitType.Hours:
                        instance.timer.Interval = new TimeSpan(interval, 0, 0);
                        break;
                    case CheckInterval.UnitType.Minutes:
                        instance.timer.Interval = new TimeSpan(0, interval, 0);
                        break;
                    case CheckInterval.UnitType.Days:
                    default:
                        instance.timer.Interval = new TimeSpan(interval, 0, 0, 0);
                        break;
                }
            }
            Debug.WriteLine(String.Format("[CreateUpdateCheker {0}] Interval configured {1} {2}", DateTime.Now, interval, unit.ToString()));
        }

        public static void UpdateInterval(int interval)
        {
            UpdateInterval(interval, CheckInterval.UnitType.Days);
        }
        public static void UpdateInterval(int interval, CheckInterval.UnitType unit)
        {
            if (instance != null)
            {

                instance.timer.Stop();

                if (interval > 0)
                {
                    switch (unit)
                    {
                        case CheckInterval.UnitType.Hours:
                            instance.timer.Interval = new TimeSpan(interval, 0, 0);
                            break;
                        case CheckInterval.UnitType.Minutes:
                            instance.timer.Interval = new TimeSpan(0, interval, 0);
                            break;
                        case CheckInterval.UnitType.Days:
                        default:
                            instance.timer.Interval = new TimeSpan(interval, 0, 0, 0);
                            break;
                    }
                    instance.timer.Start();
                }

                Debug.WriteLine(String.Format("[UpdateInterval {0}] Interval updated {1} {2}", DateTime.Now, interval, unit.ToString()));
            }
        }

        private void CheckUpdate(object sender, EventArgs e)
        {
            Utils.AppUtil.CheckUpdate();
            instance.lastChecked = DateTime.Now;
        }

        public static void Disposed()
        {
            instance.timer.Stop();
            instance.timer = null;
        }
    }
}
