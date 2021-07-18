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
                if (!this.isLoggedIn)
                    Console.WriteLine("Error, username and/or password were incorrect, please try again");
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
            //get viable timeframe
            string timeframeFrom;
            string timeframeTo;
            bool succ;
            do
            {
                timeframeFrom = HelperMethods.CreateDateTimeFromUserInput("Timeframe from");
                timeframeTo = HelperMethods.CreateDateTimeFromUserInput("Timeframe to");
                succ = isTimeframeViable(timeframeFrom, timeframeTo);
                if (!succ)
                    Console.WriteLine("Error, start time has to be before or equal to end time");
            } while (!succ);
            //get user input: How many vaccinations should be parallel
            int parallelVaccinations = 0;
            Exception error = new Exception();
            do
            {
                try
                {
                    Console.WriteLine("parallel vaccinations:");
                    parallelVaccinations = int.Parse(Console.ReadLine());
                    if (parallelVaccinations <= 0)
                        throw new Exception("input has to be above 0");
                    else
                    {
                        error = null;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("your input has to be an int above 0, please try again");
                    error = e;
                }
            } while (error != null);
            //get user input: How much difference should be between vacc appointments
            int timeInterval = 0;
            do
            {
                try
                {
                    Console.WriteLine("time interval in minutes:");
                    timeInterval = int.Parse(Console.ReadLine());
                    error = null;
                }
                catch (Exception e)
                {
                    Console.WriteLine("your input has to be an int, please try again");
                    error = e;
                }
            } while (error != null);
            
            //compute how many appointments have to be created
            //Bugfix: when time between Appointments is set to 0 an error gets created since you would divide by zero, so i set the timeInterval to the difference between timeframe start and timeframe end
            if (timeInterval <= 0 && HelperMethods.DifferenceBetweenTimesInMinutes(timeframeFrom, timeframeTo) > 0)
            {
                timeInterval = HelperMethods.DifferenceBetweenTimesInMinutes(timeframeFrom, timeframeTo);
            } //if the difference is 0, you can simply set timeInterval to a random positive int
            else
            {
                timeInterval = int.MaxValue;
            }
            int totalAppointmentsToCreate = HelperMethods.DifferenceBetweenTimesInMinutes(timeframeFrom, timeframeTo) /
                timeInterval * parallelVaccinations;
            //Bugfix: if the difference between the timeframe start and timeframe end is 0, no appointment gets created. But since it's a viable option to say i only want appointments 
            //on this time exactly i set the totalAppointmentsToCreate to the amount of parallel appointments
            if (totalAppointmentsToCreate == 0)
            {
                totalAppointmentsToCreate = parallelVaccinations;
            }
            for (int i = 0; i < totalAppointmentsToCreate; i++)
            {
                //time from the beginning of the appointments + computed time passed since then
                int minutesPassed = (i / parallelVaccinations) * timeInterval;
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
            Day validDay = null;
            try
            {
                validDay = appointmentManager.GetDay(day);
            }
            catch (Exception e)
            {
                Console.WriteLine("There are no appointments (neither free nor taken) on this day");
                return;
            }
            if (validDay.CalculatePercentageTaken() < 100)
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

        private bool isTimeframeViable(string timeframe1, string timeframe2)
        {
            var time1 = HelperMethods.CreateDateTimeFromFormattedString(HelperMethods.PlaceholderDate, timeframe1);
            var time2 = HelperMethods.CreateDateTimeFromFormattedString(HelperMethods.PlaceholderDate, timeframe2);
            if (DateTime.Compare(time1, time2) <= 0)
                return true;
            return false;
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