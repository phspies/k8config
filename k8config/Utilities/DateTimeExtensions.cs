using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8config.Utilities
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates the age in years of the current System.DateTime object today.
        /// </summary>
        /// <param name="birthDate">The date of birth</param>
        /// <returns>Age in years today. 0 is returned for a future date of birth.</returns>
        public static int Age(this DateTime birthDate)
        {
            return Age(birthDate, DateTime.Today);
        }

        /// <summary>
        /// Calculates the age in years of the current System.DateTime object on a later date.
        /// </summary>
        /// <param name="birthDate">The date of birth</param>
        /// <param name="laterDate">The date on which to calculate the age.</param>
        /// <returns>Age in years on a later day. 0 is returned as minimum.</returns>
        public static int Age(this DateTime birthDate, DateTime laterDate)
        {
            int age;
            age = laterDate.Year - birthDate.Year;

            if (age > 0)
            {
                age -= Convert.ToInt32(laterDate.Date < birthDate.Date.AddYears(age));
            }
            else
            {
                age = 0;
            }

            return age;
        }
        public static string GetPrettyDate(DateTime d)
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(d);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;
            int weekDiff = (int)s.TotalDays/7;
            int monthDiff = (int)s.TotalDays / 31;
            int yearDiff = weekDiff / 53;

            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0)
            {
                return null;
            }


            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 minute ago";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} minutes",
                        Math.Floor((double)secDiff / 60));
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} hours ago",
                        Math.Floor((double)secDiff / 3600));
                }
            }
            // 6.
            // Handle previous days.

            if (yearDiff != 0)
            {
                if (yearDiff == 1)
                {
                    return $"{yearDiff} year";
                }
                else
                {
                    return $"{yearDiff} years";
                }
            }
            if (monthDiff != 0)
            {
                if (monthDiff == 1)
                {
                    return $"{monthDiff} month";
                }
                else
                {
                    return $"{monthDiff} months";
                }
            }
            if (weekDiff != 0)
            {
                if (weekDiff == 1)
                {
                    return $"{weekDiff} week";
                }
                else
                {
                    return $"{weekDiff} weeks";
                }
            }
            if (dayDiff != 0)
            {
                if (dayDiff == 1)
                {
                    return "yesterday";
                }
                else
                {
                    return $"{dayDiff} days";
                }    
            }
            return null;
        }
    }
}
