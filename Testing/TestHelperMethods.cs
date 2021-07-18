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
            int diff = HelperMethods.DifferenceBetweenTimesInMinutes("12:30", "13:30");
            Assert.AreEqual(diff, 60);
        }
    }
}