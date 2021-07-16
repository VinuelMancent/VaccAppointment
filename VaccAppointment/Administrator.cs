using System;
using ConsoleManager;

namespace VaccAppointment
{
    public class Administrator
    {
        private AppointmentManager appointmentManager = AppointmentManager.GetInstance();
        private string username = "vincent";
        private string password = "hatne1verdient";
        private bool isLoggedIn = false;
        public void Run()
        {
            ConsoleManager.ConsoleManager cm = new ConsoleManager.ConsoleManager("Welcome Admin, you have the following options:", "admin");
            cm.AddOption(new Option("Login", "1", Login));
            cm.AddOption(new Option("Create new appointment", "2", AddNewAppointment));
            cm.AddOption(new Option("Show info on specific day", "3", ShowInformationOneDay));
            cm.AddOption(new Option("Show info on all days", "4", () => ShowInformationAllDays()));
            cm.Run();
        }

        private void Login()
        {
            do
            {
                Console.WriteLine("Username (not casesensitive):");
                string usernameInput = Console.ReadLine();
                Console.WriteLine("Password (casesensitive):");
                string passwordInput = Console.ReadLine();
                Login(usernameInput, passwordInput);
            } while (!this.isLoggedIn);
            Console.WriteLine("You are successfully logged in!");
        }
        private void Login(string username, string password)
        {
            if(username.ToLower() == this.username.ToLower() && password == this.password)
                this.isLoggedIn = true;
        }

       
        private void AddNewAppointment()
        {
            if (!isLoggedIn)
            {
                Console.WriteLine("Please login first");
                return;
            }
            var day = HelperMethods.CreateDayFromUserInput("On which day should the new appointment(s) be?");
            //check if day is already available
            appointmentManager.AddDay(day, true);
            //
            Console.WriteLine("Timeframe from (format: hh:mm):");
            string timeframeFrom = Console.ReadLine();
            Console.WriteLine("Timeframe to (format: hh:mm):");
            string timeframeTo = Console.ReadLine();
            Console.WriteLine("parallel vaccinations:");
            string parallelVaccinations = Console.ReadLine();
            Console.WriteLine("time interval in minutes:");
            string timeInterval = Console.ReadLine();
            //compute how many appointments have to be created
            int totalAppointmentsToCreate = HelperMethods.DifferenceBetweenTimesInMinutes(timeframeFrom, timeframeTo) /
                int.Parse(timeInterval) * int.Parse(parallelVaccinations);
            for (int i = 0; i < totalAppointmentsToCreate; i++)
            {
                //time from the beginning of the appointments + computed time passed since then
                int minutesPassed = (i / int.Parse(parallelVaccinations)) * int.Parse(timeInterval);
                DateTime vaccTime = HelperMethods.CreateDateTimeFromFormattedString(day.Date, timeframeFrom).AddMinutes(minutesPassed);
                Appointment app = new Appointment(day.Date, vaccTime.ToShortTimeString());
                appointmentManager.AddAppointmentOnDay(day, app);
            }
            //WaitingList should get worked here
            WaitingList.GetInstance().WorkThroughList();
            appointmentManager.SerializeAppointments();
            Console.WriteLine("success");
        }

        private void ShowInformationOneDay()
        {
            if (!isLoggedIn)
            {
                Console.WriteLine("Please login first");
                return;
            }
            var day = HelperMethods.CreateDayFromUserInput();
            ShowInformationOneDay(day);
        }
        private void ShowInformationOneDay(Day day)
        {
            if (appointmentManager.GetDay(day).CalculatePercentageTaken() < 100)
            {
                ConsoleManager.ConsoleManager cm = new ConsoleManager.ConsoleManager("choose if you want to ", "");
                cm.AddOption(new Option("ShowOccupiedPercentage", "1",() => ShowOccupiedPercentage(day), 
                    "shows how many percent of appointments are taken", "" ));
                cm.AddOption(new Option("ShowFreeAppointments", "2",
                    () => ShowFreeAppointments(day), "shows all available appointments on the chosen day", ""));
                cm.Run(false);
            }
            else
            {
                Console.WriteLine($"On {day.Date} 100% of the appointments are already taken");
            }
        }

        private void ShowInformationAllDays()
        {
            if (!isLoggedIn)
            {
                Console.WriteLine("Please login first");
                return;
            }
            //show how many appointments exist and how many of them are free/given
            showAllAppointments();
            ConsoleManager.ConsoleManager cm = new ConsoleManager.ConsoleManager("choose if you want to", "");
            //show how many free appointments already happend 
            cm.AddOption(new Option("ShowAlreadyHappenedFreeAppointments", "1", ShowPastFreeAppointments,
                "shows how many free appointments are in the past", ""));
            //show how many free appointments are in the future
            cm.AddOption(new Option("ShowFutureFreeAppointments", "2", showFutureFreeAppointments,
                "shows how many free appointments are in the future", ""));
            cm.Run(false);
        }

        private void ShowOccupiedPercentage(Day day)
        {
            Console.WriteLine(appointmentManager.GetDay(day).CalculatePercentageTaken() + "% of the appointments are taken");
        }

        private void ShowFreeAppointments(Day day)
        {
            var freeApps = appointmentManager.GetDay(day).GetFreeAppointmentTimes();
            foreach (var freeApp in freeApps)
            {
                Console.WriteLine($"{freeApp.Key} ({freeApp.Value})");
            }
        }

        private void showAllAppointments()
        {
            var allAppointments = appointmentManager.CountAllAppointments();
            var allFreeAppointments = appointmentManager.CountAllFreeAppointments();
            Console.WriteLine($"Total appointments: {allAppointments}, {allFreeAppointments} from them are free, {allAppointments - allFreeAppointments} are given");
        }

        private void ShowPastFreeAppointments()
        {
            Console.WriteLine($"Amount of past free appointments: {appointmentManager.CountPastFreeAppointments()}");
        }

        private void showFutureFreeAppointments()
        {
            Console.WriteLine($"Amount of future free appointments: {appointmentManager.CountFutureFreeAppointments()}");
        }
        
    }
}