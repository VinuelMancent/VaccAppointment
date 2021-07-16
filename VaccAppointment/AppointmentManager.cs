using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace VaccAppointment
{
    public class AppointmentManager
    {
        private static AppointmentManager instance;
        private List<Day> days = new List<Day>();
        private static List<INotifcationObserver> notifyObserver = new List<INotifcationObserver>();

        public void SerializeAppointments()
        {
            SortAppointmentList();
            StringBuilder sb = new();
            sb.Append("{Appointments:\n");
            sb.Append("[\n");
            foreach (var day in days)
            {
                sb.Append(day.SerializeFio());
                sb.Append(",\n");
            }
            sb.Append("]}\n");
            Kubernative.Fio.File file = new Kubernative.Fio.File();
            file.Reader.Read(Encoding.ASCII.GetBytes(sb.ToString()), "json");
            file.Writer.Write("appointments.json", "json");
        }

        public void DeserializeAppointments()
        {
            Kubernative.Fio.File file = new Kubernative.Fio.File();
            file.Reader.Read("appointments.json", "json");
            //Ab hier: die liste von days dynamisch füllen
            
            var dayList = file.Tree.GetSubTreeList("Appointments");
            foreach (var dayElement in dayList)
            {
                var dayTree = dayElement.Value;
                string dateInput = dayTree.GetElement("Date");
                Day day = HelperMethods.CreateDayFromFormattedString(dateInput);
                var appTree = dayTree.GetSubTreeList("Appointments");
                foreach (var appElement in appTree)
                {
                    string isGivenInput = appElement.Value.GetElement("IsGiven");
                    string uuidInput = appElement.Value.GetElement("Uuid");
                    string timeInput = appElement.Value.GetElement("Time");
                    string mailInput = appElement.Value.GetElement("Mail");
                    Appointment appointment = new Appointment(bool.Parse(isGivenInput), uuidInput,dateInput,  timeInput, mailInput);
                    day.AddAppointment(appointment);
                }
                AddDay(day, true);
            }
            SortAppointmentList();
        }

        #region Days
        public bool AddDay(Day day, bool SkipSerialisation = false)
        {
            foreach (var element in days)
            {
                if (day.Date.Equals(element.Date))
                {
                    return true;
                }
            }
            days.Add(day);
            //Add the data into the json file (if it isnt just getting loaded from there...)
            if (!SkipSerialisation)
            {
                SortAppointmentList();
                SerializeAppointments();
            }
            return false;
        }

        //checks if the day already exists in the List of days and returns it if existing
        public Day GetDay(Day day)
        {
            foreach (var dayElement in days)
            {
                if (dayElement.Date.Equals(day.Date))
                    return dayElement;
            }
            return null;
        }
        #endregion
        
        #region Appointment
        public void AddAppointmentOnDay(Day day, Appointment appointment)
        {
            AddDay(day);
            var existingDay = GetDay(day);
            existingDay.AddAppointment(appointment);
        }

        public bool GiveAppointment(string UUID, string mail, User user)
        {
            foreach (var day in days)
            {
                foreach (var app in day.Appointments)
                {
                    if (app.Uuid == UUID && !app.IsGiven)
                    {
                        app.Give(mail);
                        notify(day, app, user);
                        SerializeAppointments();
                        return true;
                    }
                }
            }
            return false;
        }

        public int CountAllAppointments()
        {
            int allAppointments = 0;
            foreach (var day in days)
            {
                allAppointments += day.CountAllAppointments();
            }
            return allAppointments;
        }

        public int CountPastFreeAppointments()
        {
            int freePastAppointments = 0;
            foreach (var day in days)
            {
                foreach (var app in day.Appointments)
                {
                    if (!app.IsInFuture())
                        freePastAppointments++;
                }
            }
            return freePastAppointments;
        }

        public int CountFutureFreeAppointments()
        {
            int freeFutureAppointments = 0;
            foreach (var day in days)
            {
                foreach (var app in day.Appointments)
                {
                    if (app.IsInFuture())
                        freeFutureAppointments++;
                }
            }
            return freeFutureAppointments;
        }

        //prints all Free Future Appointments
        public void ShowFutureAppointments()
        {
            foreach (var day in days)
            {
                foreach (var app in day.Appointments)
                {
                    if (app.IsInFuture() && !app.IsGiven)
                    {
                        Console.WriteLine("-----------------"); 
                        Console.WriteLine($"Day: {day.Date}");
                        Console.WriteLine($"Time: {app.Time}");
                        Console.WriteLine($"ID: {app.Uuid}");
                    }
                }
            }
        }

        public string GetClosestFreeFutureAppointment()
        {
            foreach (var day in days)
            {
                foreach (var app in day.Appointments)
                {
                    if (app.IsInFuture())
                    {
                        return app.Uuid;
                    }
                }
            }
            return String.Empty;
        }
        public int CountAllFreeAppointments()
        {
            int givenAppointments = 0;
            foreach (var day in days)
            {
                givenAppointments += day.CountFreeAppointments();
            }
            return givenAppointments;
        }
        #endregion

        private void notify(Day day, Appointment appointment, User user)
        {
            foreach (var obs in notifyObserver)
            {
                obs.Notify(day, appointment, user);
            }
        }
        public static AppointmentManager GetInstance()
        {
            if (instance == null)
            {
                instance = new AppointmentManager();
                notifyObserver.Add(new NotificationMailObserver());
                notifyObserver.Add(new NotificationStdoutObserver());
            }
            return instance;
        }

        private void SortAppointmentList()
        {
            days.Sort((x, y) => DateTime.Compare(HelperMethods.CreateDateTimeFromFormattedString(x.Date), HelperMethods.CreateDateTimeFromFormattedString(y.Date)));
        }
    }
}