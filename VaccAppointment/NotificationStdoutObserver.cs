using System;

namespace VaccAppointment
{
    public class NotificationStdoutObserver: INotifcationObserver
    {
        public void Notify(Day day, Appointment appointment, User user)
        {
            Console.WriteLine("The following appointment was given successfully:");
            Console.WriteLine($"Appointment: date: {day.Date}, time: {appointment.Time}, id: {appointment.Uuid}");
            Console.WriteLine($"User: mail: {user.mail}, name: {user.surname}, {user.prename}");
        }
    }
}