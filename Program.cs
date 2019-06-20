using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic; 
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace temp
{
    class Program
    {
        static void Main(string[] args)
        {

            List<Tuple<string,int>> list = new List<Tuple<string, int>>();

            list.Add(new Tuple<string, int>("b", 1));
            list.Add(new Tuple<string, int>("a", 3));
            list.Add(new Tuple<string, int>("c", 2));

            var res = list.Find(a => a.Item1 == "b");

            if(res == null)
                System.Console.WriteLine("Not Found");
            else
                System.Console.WriteLine("Found");

            Dictionary<string,int> dict = new Dictionary<string, int>();
            dict.Add("b", 1);
            dict.Add("a", 3);
            dict.Add("c", 2);

        }
    }
}
