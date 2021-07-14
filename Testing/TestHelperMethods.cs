using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VaccAppointment;

namespace Testing
{
    [TestClass]
    public class TestHelperMethods
    {
       
        [TestMethod]
        public void TestDifferenceBetweenTimesInMinutes()
        {
            Console.WriteLine(HelperMethods.DifferenceBetweenTimesInMinutes("12:30", "22:30"));
        }
    }
}