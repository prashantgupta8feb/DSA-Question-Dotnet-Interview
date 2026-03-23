using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public static class CheckPalindrome
    {
        public static bool CallPalindrome()
        {
            bool res = isPalindrome("racecar");
            return res;
        }

        public static bool isPalindrome(string input)
        {
            if (input == null) return false;
            else
            {
                input = input.Replace(" ", "");
                input = input.ToLower();
                Console.WriteLine(input);
                int left = 0, right = input.Length - 1;
                while (left < right)
                {
                    if (input[left] != input[right]) return false;
                    left++;
                    right--;
                }
                return true;
            }
        }
    }
}
