using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public static class LengthOfLongestSubstring
    {

        public static void CallLongestSubstringWithoutRepetition()
        {
            int res = LongestSubstringWithoutRepetition("bbabcdefgbbb");
            Console.WriteLine($"{res}");
        }
        public static int LongestSubstringWithoutRepetition(string s)
        {
            Dictionary<char, int> map = new Dictionary<char, int>();

            // Left pointer of sliding window
            int left = 0;

            // Stores maximum substring length found so far
            int maxLength = 0;

            // Right pointer expands the window
            for (int right = 0; right < s.Length; right++)
            {
                // Current character at right pointer
                char current = s[right];

                // If character already exists in dictionary,
                // move left pointer to avoid repetition
                if (map.ContainsKey(current))
                {
                    // Move left only forward
                    // map[current] + 1 => next position after duplicate
                    left = Math.Max(left, map[current] + 1);
                }

                // Update current character's latest index
                map[current] = right;

                // Calculate current window length
                int currentLength = right - left + 1;

                // Update maximum length if current window is larger
                maxLength = Math.Max(maxLength, currentLength);
            }

            // Return longest substring length without repeating characters
            return maxLength;
        }

    }
}
