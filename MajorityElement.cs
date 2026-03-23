using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public class MajorityElement
    {
        public static void CallMajorityElement()
        {
            int[] arr = { 2, 1, 1, 1, 1, 2, 2 };
            int majority = FindMajorityElement(arr);
            Console.WriteLine($"The majority element is: {majority}");
        }
        public static int FindMajorityElement(int[] nums)
        {
            Dictionary<int, int> frequencyDict = new Dictionary<int, int>();
            foreach (int num in nums)
            {
                if (frequencyDict.ContainsKey(num))
                {
                    frequencyDict[num]++;
                }
                else
                {
                    frequencyDict[num] = 1;
                }
            }
            int majorityElement = nums[0];
            int maxFrequency = frequencyDict[majorityElement];
            foreach (var elem in frequencyDict)
            {
                if (elem.Value > maxFrequency)
                {
                    majorityElement = elem.Key;
                    maxFrequency = elem.Value;
                }
            }
            return majorityElement;
        }
    }
}
