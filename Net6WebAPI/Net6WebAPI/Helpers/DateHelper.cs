namespace Net6WebAPI.Helpers
{
    public class DateHelper
    {
        public static IEnumerable<string> GenerateDateSequence(DateTime startDate, DateTime endDate)
        {
            List<string> dateSequence = new List<string>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dateSequence.Add(date.ToString("MM/dd/yyyy"));
            }

            return dateSequence;
        }

        public static bool IsValidDateRange(DateTime startDate, DateTime endDate)
        {
            // Calculate the difference in days
            int dayDifference = (endDate - startDate).Days;

            // Check if the difference is 15 (this means a range of 16 days including the start and end dates)
            return dayDifference <= 15;
        }
    }
}
