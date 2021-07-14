using System;

namespace ConsoleManager
{
    public class Option
    {
        private readonly string _name;
        private readonly string _command;
        private readonly string _description;
        private readonly string _help;
        private readonly Action act;
       

        public Option(string name, string command,  Action act, string description = "", string help = "")
        {
            this._name = name;
            this._command = command;
            this._description = description;
            this._help = help;
            this.act = act;
        }
        TResult Test<TResult>(Delegate f, params object[] args)
        {
            var result = f.DynamicInvoke(args);
            return (TResult)Convert.ChangeType(result, typeof(TResult));
        }
        
        //prints out the specific option text
        public void Print(bool description = false)
        {
            string toPrint;
            if(description)
                toPrint = String.Format("{0,-20} {1,-20} {2,-50}", $"<{this._command}>", this._name, this._description );
            else
            {
                toPrint = String.Format("{0,-20} {1,-20}", $"<{this._command}>", this._name);
            }
            Console.WriteLine(toPrint);
        }

        public void PrintDescription()
        {
            Console.WriteLine(_description);
        }
        public void PrintHelp()
        {
            Console.WriteLine(_help);
        }

        public string GetName()
        {
            return this._name;
        }

        public string GetCommand()
        {
            return this._command;
        }

        public void Run()
        {
            this.act();
            if(this.act == null)
                throw new Exception("got no function :( ");
        }
    }
}