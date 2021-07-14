using System;
using System.Collections.Generic;


namespace ConsoleManager
{
    public class ConsoleManager
    {
        private readonly List<Option> _options;
        private readonly string _description;
        private readonly string menuName;

        private Exit exit = new();
        
        public ConsoleManager(string description, string menuname)
        {
            this._options = new List<Option>();
            this._description = description;
            this.menuName = menuname;
        }
        //Adds a new option to the option list, returns true if success, false if error
        public bool AddOption(Option option)
        {
            try
            {
                _options.Add(option);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        //Prints out all Option texts
        public void PrintOptions()
        {
            foreach (var option in _options)
            {
                option.Print();
            }
        }
        //Prints the description to the project
        public void PrintDescription()
        {
            Console.WriteLine(_description);
        }
        
        //handles input as well as logic of the possible loop
        public void Run(bool repeat = true)
        {
            //check if the exit option is available
            ExitLastOption();
            var again = false;
            //first time: print description as well as options
            PrintDescription();


            do
            {
                //Console.WriteLine("\n {0,-20} {1,-20} {2,-50}", "name","command", "description");
                //Console.WriteLine("---------------------------------------------------------------------------");
                PrintOptions();
                CallCommand();
                if(repeat)
                    again = askIfAgain();
            } while (again);
        }
        
        //checks if last option is "exit", if not, add exit as last option
        private void ExitLastOption()
        {
            if (!_options[_options.Count - 1].GetName().Equals("Exit"))
            {
                AddOption(new Option("Exit", "exit", () => exit.KillProgramm() ,"Exits the program", "...simply exits the program"));
            }
        }

        private void  CallCommand()
        {
            Console.WriteLine("Which command do you want to use?");
            string answer = Console.ReadLine();
            foreach (var option in _options)
            {
                if (option.GetCommand().ToLower().Equals(answer.ToLower()))
                {
                    option.Run();
                }
            }
            
        }
        //helpermethod to check, whether to run the mainloop again or not
        private bool askIfAgain()
        {
            Console.WriteLine($"Do you want to go to the {menuName} menu again? (y/n)");
            while (true)
            {
                var answer = Console.ReadLine();
                switch (answer.ToLower())
                {
                    case "y":
                        return true;
                    case "n":
                        return false;
                    default:
                        Console.WriteLine($"{answer} is not a viable option, please answer with \"y\" or \"n\"");
                        break;
                }
            }
        }

        private void Exit()
        {
            Environment.Exit(0);
        }
        
    }
}