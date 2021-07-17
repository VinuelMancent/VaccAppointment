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
        private Kubernative.Fio.File userListFile;
        private IList<User> userOnWaitingList = new List<User>();
        private IList<User> userOnUserList = new List<User>();
        private AppointmentManager appointmentManager = AppointmentManager.GetInstance();


        private WaitingList()
        {
            waitingListFile = new();
            userListFile = new();
        }
        public void Deserialize()
        {
            //check if userListFile is available
            //if yes --> Load UserListfile, go through every User and load him into a list
            //if no --> initialize userOnUserList as empty list
            if (File.Exists("user.json"))
            {
                userListFile.Reader.Read("user.json");
                var currUserOnUserList = userListFile.Tree.GetList("User");
                foreach (JObject userElement in currUserOnUserList)
                {
                    User user = new User((string)userElement["prename"],(string)userElement["surname"],"",(string)userElement["birthday"],(string)userElement["mail"],(string)userElement["address"],(string)userElement["phonenumber"]);
                    this.AddUserListUser(user);
                }
            }
            else
            {
                this.userOnUserList = new List<User>();
            }

            //check if waitingListFile is available
            //if yes --> Load waitingListFile, go through every user and load him into a list
            //if no --> create waitingList.json, initialize userOnWaitingList as empty List
            if (File.Exists("waitingList.json"))
            {
                waitingListFile.Reader.Read("waitingList.json");
                var currUserOnWaitingList = waitingListFile.Tree.GetList("User");
                foreach (JObject userElement in currUserOnWaitingList)
                {
                    User user = new User((string)userElement["prename"],(string)userElement["surname"],"",(string)userElement["birthday"],(string)userElement["mail"],(string)userElement["address"],(string)userElement["phonenumber"]);
                    this.userOnWaitingList.Add(user);
                }
            }
            else
            {
                this.userOnWaitingList = new List<User>();
                Serialize();
            }
            
        }

        public void AddUserListUser(User user)
        {
            this.userOnUserList.Add(user);
        }

        public void AddWaitingListUser(User user)
        {
            bool alreadyExists = false;
            //check if user is already on waitingList
            foreach (var userElement in userOnWaitingList)
            {
                if (userElement.GetUserEmail() == user.GetUserEmail())
                    alreadyExists = true;
            }
            //check if user is already on userList (means he already got an appointment)
            foreach (var userElement in userOnUserList)
            {
                if (userElement.GetUserEmail() == user.GetUserEmail())
                    alreadyExists = true;
            }
            if (!alreadyExists)
                this.userOnWaitingList.Add(user);
            else
            {
                Console.WriteLine("Error: user already got an appointment or is on the waiting list");
            }

        }
        public void Serialize()
        {
            IList<dynamic> WaitingListList = new List<dynamic>();
            foreach (var element in userOnWaitingList)
            {
                WaitingListList.Add(element);
            }
            waitingListFile.Tree.SetList("User", WaitingListList);
            waitingListFile.Writer.Write("waitingList.json", "json");
        }

        public void WorkThroughList()
        {
            List<User> toRemove = new();
            foreach (var element in userOnWaitingList)
            {
                string appointmentUuid = appointmentManager.GetClosestFreeFutureAppointment();
                if (appointmentUuid == String.Empty)
                    return;
                bool succ = appointmentManager.GiveAppointment(appointmentUuid, element.mail, element);
                if(succ) 
                    toRemove.Add(element);
            }

            foreach (var toRemoveElement in toRemove)
            {
                userOnWaitingList.Remove(toRemoveElement);
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