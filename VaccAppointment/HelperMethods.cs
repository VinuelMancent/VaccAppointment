using System;
using System.Text.RegularExpressions;

namespace VaccAppointment
{
    public class HelperMethods
    {
        public static string PlaceholderDate = "01.01.1960";
        public static string[] PlaceholderDateArray = {"01", "01", "1960"};
        public static string NamePattern = @"^[a-zA-Z]+$";
        public static string MailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" // local-part
                + "@"
                + @"((([\w]+([-\w]*[\w]+)*\.)+[a-zA-Z]+)|" // domain
                + @"((([01]?[0-9]{1,2}|2[0-4][0-9]|25[0-5]).){3}[01]?[0-9]{1,2}|2[0-4][0-9]|25[0-5]))\z";
        public static string PhonenumberPattern = @"^[0-9]+(\/[0-9]+)*$";
       

        public static string GetAndValidateUserInput(string message, string regex)
        {
            string userInput;
            do
            {
                Console.WriteLine(message);
                userInput = Console.ReadLine();
            } while (Regex.IsMatch(regex, userInput));

            return userInput;
        }

        public static string CreateDateTimeFromUserInput(string message)
        {
            Exception exception = new Exception();
            string res = string.Empty;
            do
            {
                try
                {
                    Console.WriteLine($"{message} (Format: hh:mm):");
                    string userInput = Console.ReadLine();
                    var userInputSplit = userInput.Split(":");
                    DateTime time = new DateTime(int.Parse(PlaceholderDateArray[2]), int.Parse(PlaceholderDateArray[1]),
                        int.Parse(PlaceholderDateArray[0]), int.Parse(userInputSplit[0]), int.Parse(userInputSplit[1]),
                        0);
                    res = time.ToShortTimeString();
                    exception = null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: your input format was incorrect, please try again");
                    exception = e;
                }
            } while (exception != null);
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
        //Creates a datetime only with date information
        public static DateTime CreateDateTimeFromFormattedString(string formatted)
        {
            //string format: dd.mm.yyyy
            var split = formatted.Split(".");
            DateTime res = new DateTime(int.Parse(split[2]),int.Parse(split[1]),int.Parse(split[0]));
            return res;
        }
        public static Day CreateDayFromUserInput()
        {
            string dayInput;
            Day res = null;
            Exception error = new();
            do
            {
                try
                {
                    Console.WriteLine("From which day do you need Information? (Format: dd.mm.yyyy)");
                    dayInput = Console.ReadLine();
                    res = CreateDayFromFormattedString(dayInput);
                    error = null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Your input was no correct date, please try again");
                    error = e;
                }
            } while (error != null);
           
           
            return res;
        }
        public static Day CreateDayFromUserInput(string message)
        {
            string dayInput = string.Empty;
            Exception error = new();
            Day dayResult = null;
            do
            {
                try
                {
                    Console.WriteLine($"{message} (Format: dd.mm.yyyy)");
                    dayInput = Console.ReadLine();
                    dayResult = CreateDayFromFormattedString(dayInput);
                    error = null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: Your day format was wrong, please try again");
                    error = e;
                }
            } while (error != null);
            return dayResult;
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
            var date1 = CreateDateTimeFromFormattedString(PlaceholderDate, time1);
            var date2 = CreateDateTimeFromFormattedString(PlaceholderDate, time2);
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