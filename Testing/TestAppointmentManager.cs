using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VaccAppointment;

namespace Testing
{
    [TestClass]
    public class TestAppointmentManager
    {
        /*
        [TestMethod]
        public void TestSerialisation()
        {
            AppointmentManager am = new AppointmentManager();
            var day = new Day(DateTime.Now);
            //
            int[] dayConverted = {2021, 12,29};
            DateTime date = new DateTime(dayConverted[0], dayConverted[1], dayConverted[2]);
            Day day2 = new Day(date);
            day2.AddAppointment(new Appointment());
            //
            day.AddAppointment(new Appointment());
            am.AddDay(day);
            am.AddDay(day2);
            am.Serialize();
        }*/
        
        /*
        [TestMethod]
        public void TestDeserialisation()
        {
            AppointmentManager am = new AppointmentManager();
            am.Deserialize();
        }*/
    }
}