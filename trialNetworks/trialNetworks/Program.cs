using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net.Http;
using System.IO;
using trialNetworks.Classes;
 
namespace trialNetworks
{
    class Program
    {
        public static bool readyToUse = false;
        static void Main(string[] args)
        {
            string again = "";
            do
            {
                Console.Clear();
                Console.WriteLine("please enter the command");
                string command = Console.ReadLine();
                string[] parts = command.Split(' ');
                if (parts[0].ToLower() == "wget" && parts.Length == 3)
                {
                    
                    string url = parts[1];
                    string dir = parts[2];
                    Console.WriteLine("did you enter the save dir including filename ? 1 or 0");
                    bool savedir;
                    if (Console.ReadLine()[0] == '1')
                        savedir = true;
                    else
                        savedir = false;
                    MyRequest re = new MyRequest(url, dir, savedir);
                    re.connect(80);
                    re.start();
                }
                else
                {
                    Console.WriteLine("you entered invalid command");
                }
                Console.WriteLine("do you want to retry ? yes or no");
                again = Console.ReadLine().ToLower();
            } while (again == "yes");
            
            
        }
        
    }
}
