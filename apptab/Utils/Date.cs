using System;

namespace apptab.Utils
{
    public class Date
    {
        public static double GetDifference(DateTime? endDate, DateTime? startDate)
        {
            if (endDate == null || startDate == null)
            {
                return 0;
            }

            return Math.Ceiling(((DateTime)endDate).Subtract((DateTime)startDate).TotalDays);
        }
    }
}
