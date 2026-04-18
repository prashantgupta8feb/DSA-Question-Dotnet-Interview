using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public static class LengthOfLongestSubstring
    {

        public static int CallLongestSubstringWithoutRepetition()
        {
            int res = LongestSubstringWithoutRepetition("bbbbb");
            return res;
        }
        public static int LongestSubstringWithoutRepetition(string s)
        {
            Dictionary<char, int> map = new Dictionary<char, int>();
            int left = 0, maxLength = 0;

            for (int right = 0; right < s.Length; right++)
            {
                char current = s[right];

                if (map.ContainsKey(current))
                {
                    left = Math.Max(left, map[current] + 1);
                }

                map[current] = right;

                maxLength = Math.Max(maxLength, right - left + 1);
            }

            return maxLength;
        }

    }
}
