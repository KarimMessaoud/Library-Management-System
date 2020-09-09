using LibraryData.Models;
using System;
using System.Collections.Generic;

namespace LibraryService
{
    public class DataHelpers
    {
        public static List<string> HumanizeBizHours(IEnumerable<BranchHours> branchHours)
        {
            var hours = new List<string>();

            foreach (var item in branchHours)
            {
                var day = HumanizeDay(item.DayOfWeek);
                var openTime = HumanizeTime(item.OpenTime);
                var closeTime = HumanizeTime(item.CloseTime);

                var timeEntry = $"{day} {openTime} to {closeTime}";
                hours.Add(timeEntry);
            }

            return hours;
        }

        private static string HumanizeTime(int time)
        {
            return TimeSpan.FromHours(time).ToString("hh':'mm");
        }

        private static string HumanizeDay(int number)
        {
            return Enum.GetName(typeof(DayOfWeek), number - 1);
        }
    }
}
