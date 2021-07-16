using System;

namespace VaccAppointment
{
    public class NotificationMailObserver: INotifcationObserver
    {
        public void Notify(Day day, Appointment appointment, User user)
        {
            //this would send a mail to the user
        }
    }
}