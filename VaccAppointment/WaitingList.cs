using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace VaccAppointment
{
    public class WaitingList
    {
        private static WaitingList instance;
        private Kubernative.Fio.File waitingListFile;
        private IList<User> userList = new List<User>();
        private AppointmentManager appointmentManager = AppointmentManager.GetInstance();
       
        public void Deserialize()
        {
            waitingListFile = new();
            waitingListFile.Reader.Read("waitingList.yaml");
            var currUserList = waitingListFile.Tree.GetList("User");
            foreach (JObject userElement in currUserList)
            {
                User user = new User((string)userElement["prename"],(string)userElement["surname"],"",(string)userElement["birthday"],(string)userElement["mail"],(string)userElement["address"],(string)userElement["phonenumber"]);
                this.AddUser(user);
            }
        }

        public void AddUser(User user)
        {
            bool alreadyExists = false;
            foreach (User userElement in userList)
            {
                if (userElement.GetUserEmail() == user.GetUserEmail())
                    alreadyExists = true;
            }

            if (!alreadyExists)
                this.userList.Add(user);
            else
            {
                Console.WriteLine("Error: user is already in waitingList");
            }
        }
        public void Serialize()
        {
            Kubernative.Fio.File testfile= new();
            IList<dynamic> test = new List<dynamic>();
            foreach (var element in userList)
            {
                test.Add(element);
            }
            testfile.Tree.SetList("User", test);
            testfile.Writer.Write("waitingList.yaml", "json");
        }

        public void WorkThroughList()
        {
            List<User> toRemove = new();
            foreach (var element in userList)
            {
                string appointmentUuid = appointmentManager.GetClosestFreeFutureAppointment();
                if (appointmentUuid == String.Empty)
                    return;
                appointmentManager.GiveAppointment(appointmentUuid, element.mail, element);
                toRemove.Add(element);
            }

            foreach (var toRemoveElement in toRemove)
            {
                userList.Remove(toRemoveElement);
            }
            this.Serialize();
        }

        public static WaitingList GetInstance()
        {
            if (instance == null)
                instance = new WaitingList();
            return instance;
        }
        
    }
}