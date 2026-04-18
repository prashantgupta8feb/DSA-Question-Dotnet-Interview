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
            string input = "bababddb";
            Console.WriteLine($"Longest Palindromic Substring BruteForce: {FindLongestPalindromeBruteForce(input)}"); // aba or bab
            Console.WriteLine($"Longest Palindromic Substring Optimized : {LongestPalindrome(input)}"); // aba or bab
        }

        static string FindLongestPalindromeBruteForce(string s)
        {
            string longest = "";

            for (int i = 0; i < s.Length; i++)
            {
                for (int j = i; j < s.Length; j++)
                {
                    string sub = s.Substring(i, j - i + 1);
                    Console.Write($"{sub} ");
                    Console.WriteLine();

                    if (IsPalindrome(sub) && sub.Length > longest.Length)
                    {
                        longest = sub;
                    }
                }
            }

            return longest;
        }

        static string LongestPalindrome(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            int start = 0, maxLength = 1;

            for (int i = 0; i < s.Length; i++)
            {
                // Odd length palindrome
                ExpandAroundCenter(s, i, i, ref start, ref maxLength);

                // Even length palindrome
                ExpandAroundCenter(s, i, i + 1, ref start, ref maxLength);
            }

            return s.Substring(start, maxLength);
        }
        static void ExpandAroundCenter(string s, int left, int right, ref int start, ref int maxLength)
        {
            while (left >= 0 && right < s.Length && s[left] == s[right])
            {
                int length = right - left + 1;

                if (length > maxLength)
                {
                    maxLength = length;
                    start = left;
                }

                left--;
                right++;
            }
        }

        public static bool IsPalindrome(string str)
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



