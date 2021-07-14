using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VaccAppointment;

namespace Testing
{
    [TestClass]
    public class TestWaitingList
    {
       
        [TestMethod]
        public void TestSerialize()
        {
            WaitingList wl = WaitingList.GetInstance();
            wl.AddUser(new User("vincent", "mattes", "", "10.02.1998", "vin@mat.com", "geißb.15", "01739775822"));
            //wl.serialize();
        }
    }
}