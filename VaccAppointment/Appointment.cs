using System;
using System.Runtime.Serialization;

namespace VaccAppointment
{
    
    public class Appointment
    {
        public bool IsGiven { get; set;}
        public string Uuid { get; }
        private string date;
        public string Time { get; }
        public string Mail = String.Empty;

        public Appointment(string date, string time)
        {
            this.IsGiven = false;
            Uuid = Guid.NewGuid().ToString();
            this.date = date;
            Time = HelperMethods.CreateDateTimeFromFormattedString(HelperMethods.placeholderDate, time).ToShortTimeString();
        }

        public Appointment(bool isGiven, string uuid, string date, string time, string mail)
        {
            this.IsGiven = isGiven;
            this.Uuid = uuid;
            this.date = date;
            this.Time = time;
            this.Mail = mail;
        }

        public void Give(string userMail)
        {
            this.IsGiven = true;
            this.Mail = userMail;
        }

        public bool IsInFuture()
        {
            DateTime now = DateTime.Now;
            var appDateTime = HelperMethods.CreateDateTimeFromFormattedString(this.date, this.Time);
            int indicator = DateTime.Compare(now, appDateTime);
            if (indicator < 0)
            {
                return true;
            }
            return false;
        }

    }
}