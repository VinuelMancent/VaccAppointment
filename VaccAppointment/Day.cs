using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kubernative.Fio;

namespace VaccAppointment
{
    public class Day
    {
        public List<Appointment> Appointments;
        public string Date;
        
        public Day(DateTime date)
        {
            Appointments = new List<Appointment>();
            this.Date = date.ToShortDateString();
        }

        //Adds an appointment
        public void AddAppointment(Appointment appointment)
        {
            Appointments.Add(appointment);
        }

        //computes how many percent of that days appointments are taken
        public int CalculatePercentageTaken()
        {
            if (Appointments.Count == 0)
                return 100;
            
            int takenAppointments = 0;
            foreach (var app in Appointments)
            {
                if (app.IsGiven)
                    takenAppointments++;
            }
            double result = ((double)takenAppointments / (double)Appointments.Count)*100;
            return (int)result;
        }
        //counts how many free appointments are available
        public int CountFreeAppointments()
        {
            int freeAppointments = 0;
            foreach (var app in Appointments)
            {
                if (!app.IsGiven)
                    freeAppointments++;
            }
            return freeAppointments;
        }
        //computes how many appointments are there
        public int CountAllAppointments()
        {
            return Appointments.Count;
        }
        //returns a list of all free appointments
        public List<Appointment> GetFreeAppointments()
        {
            List<Appointment> apps = new List<Appointment>();
            foreach (var app in Appointments)
            {
                if (!app.IsGiven)
                    apps.Add(app);
            }
            return apps;
        }

        public Dictionary<string, int> GetFreeAppointmentTimes()
        {
            Dictionary<string, int> freeAppointments = new();
            foreach (var app in Appointments)
            {
                if (!app.IsGiven && freeAppointments.ContainsKey(app.Time))
                    freeAppointments[app.Time] += 1;
                else if (!app.IsGiven && !freeAppointments.ContainsKey(app.Time))
                    freeAppointments.Add(app.Time, 1);
            }
            return freeAppointments;
        }

        public string SerializeFio()
        {
            IFile file = new Kubernative.Fio.File();
            IList<dynamic> appointmentsList = new List<dynamic>();
            foreach (var app in Appointments)
            {
                appointmentsList.Add(app);
            }
            //Tree füllen
            file.Tree.SetElement("Date", Date);
            file.Tree.SetList("Appointments", appointmentsList);
            string res = System.Text.Encoding.Default.GetString(file.Writer.Serialize("json"));
            return res;
        }
        
       
    }
}