namespace VaccAppointment
{
    public interface INotifcationObserver
    {
        public void Notify(Day day, Appointment appointment, User user);
    }
}