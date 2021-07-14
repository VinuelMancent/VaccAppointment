using System;

namespace VaccAppointment
{
    public class HelperMethods
    {
        public static string placeholderDate = "01.01.1960";
        //Creates a datetime for 
        public static DateTime CreateDateTimeFromFormattedString(string formatted)
        {
            //string format: dd.mm.yyyy
            var split = formatted.Split(".");
            DateTime res = new DateTime(int.Parse(split[2]),int.Parse(split[1]),int.Parse(split[0]));
            return res;
        }

        public static DateTime CreateDateTimeFromFormattedString(string formattedDay, string formattadTime)
        {
            //formattedDay format: dd.mm.yyyy, formattedTime format: hh:mm
            var splitDay = formattedDay.Split(".");
            var splitTime = formattadTime.Split(":");
            DateTime res = new DateTime(int.Parse(splitDay[2]), int.Parse(splitDay[1]), int.Parse(splitDay[0]),
                int.Parse(splitTime[0]), int.Parse(splitTime[1]), 0);
            return res;
        }
        public static Day CreateDayFromUserInput()
        {
            Console.WriteLine("From which day do you need Information? (Format: dd.mm.yyyy)");
            string dayInput = Console.ReadLine();
            return CreateDayFromFormattedString(dayInput);
        }
        public static Day CreateDayFromUserInput(string message)
        {
            Console.WriteLine($"{message} (Format: dd.mm.yyyy)");
            string dayInput = Console.ReadLine();
            return CreateDayFromFormattedString(dayInput);
        }

        public static Day CreateDayFromFormattedString(string formatted)
        {
            var dayConverted = DateTimeStringToInt(formatted);
            DateTime date = new DateTime(dayConverted[2], dayConverted[1], dayConverted[0]);
            Day day = new Day(date);
            return day;
        }
        
        public static int[] DateTimeStringToInt(string m)
        {
            int[] res = new int[3];
            var mArray = m.Split(".");
            res[0] = int.Parse(mArray[0]);
            res[1] = int.Parse(mArray[1]);
            res[2] = int.Parse(mArray[2]);
            return res;
        }

        public static int DifferenceBetweenTimesInMinutes(string time1, string time2)
        {
            var date1 = CreateDateTimeFromFormattedString("01.01.2000", time1);
            var date2 = CreateDateTimeFromFormattedString("01.01.2000", time2);
            var res = date2.Subtract(date1);
            return res.Hours*60+res.Minutes;
        }

        public static bool IsDayInPast(string day)
        {
            DateTime givenDay = CreateDateTimeFromFormattedString(day);
            int compare = DateTime.Compare(givenDay, DateTime.Now);
            if (compare < 0)
                return true;
            return false;
        }

        public static bool IsDayInFuture(string day)
        {
            DateTime givenDay = CreateDateTimeFromFormattedString(day);
            int compare = DateTime.Compare(givenDay, DateTime.Now);
            if (compare > 0)
                return true;
            return false;
        }
    }
}