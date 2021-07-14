using System;

namespace ConsoleManager
{
    public class Exit
    {
        public void KillProgramm()
        {
            Console.WriteLine("bin im exit");
            Environment.Exit(1);
        }
    }
}