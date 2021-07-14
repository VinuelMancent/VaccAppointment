using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VaccAppointment;

namespace Testing
{
    [TestClass]
    public class TestAdministrator
    {
        private Administrator admin = new Administrator();
        
        [TestMethod]
        public void ShowInformationOneDayEmptyDay()
        {
           //admin.ShowInformationOneDay(new Day(DateTime.Today));
        }
       
    }
}