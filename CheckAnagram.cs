using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public static class CheckAnagram
    {

        public static bool CallAnagram()
        {
            bool res = isAnagram("Dirty Room", "Dormitory");
            return res;
        }
        public static bool isAnagram(string s1, string s2)
        {
            if (s1 == null || s2 == null) return false;
            //else if (s1.Length != s2.Length) return false;
            else
            {
                s1 = s1.Replace(" ", "");
                s1 = s1.ToLower();
                Console.WriteLine(s1);
                s2 = s2.Replace(" ", "");
                s2 = s2.ToLower();
                Console.WriteLine(s2);

                Dictionary<char, int> Dict = new Dictionary<char, int>();

                foreach (var elem in s1)
                {
                    if (Dict.ContainsKey(elem)) Dict[elem]++;
                    else Dict[elem] = 1;
                }

                foreach (var elem in s2)
                {
                    if (Dict.ContainsKey(elem)) Dict[elem]--;
                }

                foreach (var Pair in Dict)
                {
                    if (Pair.Value != 0) return false;
                }
            }

            return true;
        }
    }
}
