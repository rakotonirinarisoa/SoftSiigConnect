using System;

namespace apptab.Utils
{
    public class Date
    {
        public static double GetDifference(DateTime endDate, DateTime startDate)
        {
            return Math.Ceiling(endDate.Subtract(startDate).TotalDays);
        }
    }
}
