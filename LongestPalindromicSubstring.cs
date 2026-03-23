using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public class LongestPalindromicSubstring
    {
        public static void CallLongestPalindromicSubstring()
        {
            FindLongestPalindromicSubstring("ababadcrrc");
            return;
        }

        public static void FindLongestPalindromicSubstring(string input)
        {

        }

        public bool IsPalindrome(string str)
        {
            int left = 0, right = str.Length - 1;
            while (left < right)
            {
                if (str[left] != str[right]) return false;
                left++;
                right--;
            }
            return true;
        }
    }
}



