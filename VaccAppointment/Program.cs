using System;
using System.IO;
using ConsoleManager;

namespace VaccAppointment
{
    class Program
    {
        static void Main(string[] args)
        {
            AppointmentManager appointmentManager = AppointmentManager.GetInstance();
            WaitingList waitingList = WaitingList.GetInstance();
            
            //wenn eine Terminübersicht bereits vorhanden ist --> laden
            if(File.Exists("appointments.json"))
                appointmentManager.DeserializeAppointments();
            if(File.Exists("waitingList.yaml"))
                waitingList.Deserialize();
            Administrator admin = null;
            User user = null;
            ConsoleManager.ConsoleManager cm = new ConsoleManager.ConsoleManager("Welcome to VaccAppointments. Are you an Admin or a Visitor?", "main");
            cm.AddOption(new Option("Admin", "1", () => InitAndRunAdmin(ref admin)));
            cm.AddOption(new Option("User", "2", () => InitAndRunUser(ref user)));
            cm.Run(true);
        }

        private static void InitAndRunAdmin(ref Administrator admin)
        {
            admin = new Administrator();
            admin.Run();
        }

        private static void InitAndRunUser(ref User user)
        {
            user = new User();
            user.Run();
        }
    }
}