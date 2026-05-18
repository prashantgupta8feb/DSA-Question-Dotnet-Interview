using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DSA_Questions
{
    public static class GroupAnagram
    {
        public static void CallGroupAnagram()
        {
            List<IList<string>> res = GroupAnagrams();

            foreach (var item in res)
            {
                Console.WriteLine(string.Join(", ", item));
            }

            //foreach (var group in res)
            //{
            //    foreach (var word in group)
            //    {
            //        Console.Write(word + " ");
            //    }

            //    Console.WriteLine();
            //}
        }
        /*Declare a dictionary with key as sorted string and value as list of strings
        Traverse the input array
        for each elem in strs, save it and convert to a char[] array, then sort this array
        If the sorted string doesn't exists as a key, add a new List of array at this key
        If the sorted string exists as a key, add this word at in the D1[key]
        Declare a result list of list of strings and copy the D1[Values] to it
        Return the result*/
        public static List<IList<string>> GroupAnagrams()
        {
            //input = ["eat", "tea", "tan", "ate", "nat", "bat"];
            //output: [['eat', 'tea', 'ate'], ['tan', 'nat'], ['bat']] 

            string[] input = { "eat", "tea", "tan", "ate", "nat", "bat" };
            // key = sorted word
            // value = list of matching anagrams
            Dictionary<string, List<string>> dict = new();

            foreach (var word in input)
            {
                // Sort characters of current word
                string sorted = string.Concat(word.OrderBy(c => c));

                // Create list if key doesn't exist
                if (!dict.ContainsKey(sorted))
                {
                    dict[sorted] = new List<string>();
                }

                // Add original word to matching group
                dict[sorted].Add(word);
            }

            // Return grouped anagrams
            return new List<IList<string>>(dict.Values);
        }
    }
}
