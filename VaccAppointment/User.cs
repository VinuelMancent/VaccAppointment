using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ConsoleManager;
using Newtonsoft.Json.Linq;

namespace VaccAppointment
{
    public class User
    {
        public string prename, surname, uuid, birthday, mail, address, phonenumber;
        AppointmentManager appointmentManager = AppointmentManager.GetInstance();
        WaitingList waitingList = WaitingList.GetInstance();
        public User(){}

        public User(string prename, string surname, string uuid, string birthday, string mail, string address, string phonenumber)
        {
            this.prename = prename;
            this.surname = surname;
            this.uuid = uuid;
            this.birthday = birthday;
            this.mail = mail;
            this.address = address;
            this.phonenumber = phonenumber;
        }
        public void Run()
        {
            ConsoleManager.ConsoleManager cm =
                new ConsoleManager.ConsoleManager("Welcome User, you have the following options:", "user");
            bool repeat;
            if (appointmentManager.CountAllFreeAppointments() != 0)
            {
                cm.AddOption(new Option("Show appointments on Day", "1", ShowAppointmentsOnDay));
                cm.AddOption(new Option("Show Future free appointments", "2", ShowFutureFreeAppointments));
                cm.AddOption(new Option("Register for an appointment", "3", RegisterForAppointment));
                repeat = true;
            }
            else
            {
                cm.AddOption(new Option("Register in waiting list", "1", RegisterForWaitingList));
                repeat = false;
            }
                
            cm.Run(repeat);
        }
        private void ShowAppointmentsOnDay()
        {
            Day userInput = HelperMethods.CreateDayFromUserInput();
            var appList = appointmentManager.GetDay(userInput).GetFreeAppointments();
            //check if there is any free appointment on that day
            if (appList.Count == 0)
            {
                Console.WriteLine("there are no free appointments on that day, sorry");
                return;
            }
            foreach (var app in appList)
            {
                Console.WriteLine($"Time: {app.Time}");
                Console.WriteLine($"ID: {app.Uuid}");
                Console.WriteLine("-----------------");
            }
        }

        public string GetUserEmail()
        {
            return this.mail;
        }
        private void ShowFutureFreeAppointments()
        {
            appointmentManager.ShowFutureAppointments();
        }

        private void RegisterForAppointment()
        {
            Console.WriteLine("Please now give in the ID of your wanted appointment:");
            string uuidInput = Console.ReadLine();
            string mailInput =
                HelperMethods.GetAndValidateUserInput("Please now give your Mail-Address:", HelperMethods.MailPattern);
            //check if mailaddress is already used for an appointment
            if (mailInUse(mailInput))
            {
                Console.WriteLine("your mail is already in use, sorry");
                return;
            }
            string prenameInput =
                HelperMethods.GetAndValidateUserInput("Please now give your prename:", HelperMethods.NamePattern);
            string surnameInput =
                HelperMethods.GetAndValidateUserInput("Please now give your surname:", HelperMethods.NamePattern);
            string birthdayInput = getAndValidateDate();
            string phonenumberInput = HelperMethods.GetAndValidateUserInput("Please now give your phonenumber:",
                HelperMethods.PhonenumberPattern);
            Console.WriteLine("Please now give your address:");
            string addressInput = Console.ReadLine();
            //if all entries are valid: give them to the user
            setUserInfos(prenameInput, surnameInput, uuidInput, birthdayInput, mailInput, addressInput, phonenumberInput );
           
            var success = appointmentManager.GiveAppointment(uuid, mail, this);
            if (success)
            {
                serializeUser();
            }
            
        }

        private void RegisterForWaitingList()
        {
            string mailInput =
                HelperMethods.GetAndValidateUserInput("Please now give your Mail-Address:", HelperMethods.MailPattern);
            //check if mailaddress is already used for an appointment
            if (mailInUse(mailInput))
            {
                Console.WriteLine("your mail is already in use, sorry");
                return;
            }

            string prenameInput =
                HelperMethods.GetAndValidateUserInput("Please now give your prename:", HelperMethods.NamePattern);
            string surnameInput =
                HelperMethods.GetAndValidateUserInput("Please now give your surname:", HelperMethods.NamePattern);
            string birthdayInput = getAndValidateDate();
            string phonenumberInput = HelperMethods.GetAndValidateUserInput("Please now give your phonenumber:",
                HelperMethods.PhonenumberPattern);
            Console.WriteLine("Please now give your address:");
            string addressInput = Console.ReadLine();
            //if all entries are valid: give them to the user
            setUserInfos(prenameInput, surnameInput, "", birthdayInput, mailInput, addressInput, phonenumberInput );
            this.serializeUser();
            waitingList.AddWaitingListUser(this);
            waitingList.Serialize();
        }
        //prename, surname, uuid, birthday, mail, address, phonenumber;
        private void setUserInfos(string prename, string surname, string uuid, string birthday, string mail,
            string address, string phonenumber)
        {
            this.prename = prename;
            this.surname = surname;
            this.uuid = uuid;
            this.birthday = birthday;
            this.mail = mail;
            this.address = address;
            this.phonenumber = phonenumber;
        }

        private bool mailInUse(string mail)
        {
            Kubernative.Fio.File userFile = new();
            if (File.Exists("user.json"))
            {
                userFile.Reader.Read("user.json");
                var userList = userFile.Tree.GetList("User");
                foreach (JObject userElement in userList)
                {
                    if ((string)userElement["mail"] == mail)
                        return true;
                }
            }
            return false;
        }
        

        private string getAndValidateDate()
        {
            Console.WriteLine("Please give your birthday now (Format: dd.mm.yyyy):");
            string birthdayInput = Console.ReadLine();
            Exception exception = new Exception();
            DateTime resDate = new DateTime();
            do
            {
                try
                {
                    resDate = HelperMethods.CreateDateTimeFromFormattedString(birthdayInput);
                    exception = null;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{birthdayInput} is no viable birthday, please try again");
                    exception = e;
                }
            } while (exception != null);
            return resDate.ToShortDateString();
        }

       
        //saves all the user data 
        private void serializeUser()
        {
            Kubernative.Fio.File userFile = new();
            if (File.Exists("user.json"))
            {
                userFile.Reader.Read("user.json");
            }
            else
            {
                userFile.Tree.SetList("User", new List<dynamic>());
            }

            JObject newVal = new JObject();
            newVal["prename"] = prename;
            newVal["surname"] = surname;
            newVal["uuid"] = uuid;
            newVal["birthday"] = birthday;
            newVal["mail"] = mail;
            newVal["address"] = address;
            newVal["phonenumber"] = phonenumber;
            var addedList = userFile.Tree.GetList("User");
            addedList.Add(newVal);
            userFile.Tree.SetList("User", addedList);
            userFile.Writer.Write("user.json", "json");
        }
    }
}