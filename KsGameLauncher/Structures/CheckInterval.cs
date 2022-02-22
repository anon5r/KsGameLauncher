using System;

namespace KsGameLauncher.Structures
{
    internal class CheckInterval
    {
        public enum UnitType
        {
            None,
            Minutes,
            Hours,
            Days,
        };

        private UnitType unit;
        private int interval;

        public UnitType Unit { get { return unit; } }
        public int Interval { get { return interval; } }



        public CheckInterval(int value)
        {
            new CheckInterval(value, UnitType.Days);
        }


        public CheckInterval(int value, UnitType type)
        {
            interval = value;
            unit = type;
        }


        public override string ToString()
        {
            if (interval == 0)
                return Properties.Strings.CheckUpdateInterval_None;

            switch (unit)
            {
                case UnitType.Days:
                    if (interval == 1)
                        return Properties.Strings.CheckUpdateInterval_Daily;
                    else
                        return string.Format(Properties.Strings.CheckUpdateInterval_Days, interval);

                case UnitType.Hours:
                    if (interval == 1)
                        return Properties.Strings.CheckUpdateInterval_Hourly;
                    else
                        return string.Format(Properties.Strings.CheckUpdateInterval_Hours, interval);

                case UnitType.Minutes:
                    if (interval == 1)
                        return Properties.Strings.CheckUpdateInterval_Minutely;
                    else
                        return string.Format(Properties.Strings.CheckUpdateInterval_Minutes, interval);

            }

            return "Unknown";
        }
    }
}
