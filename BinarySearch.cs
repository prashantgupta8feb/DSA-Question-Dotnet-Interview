using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public class BinarySearch
    {
        public static void CallBinarySearch()
        {
            int[] nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            BinarySearchFunc(nums, 7);
            //return res;
        }

        public static void BinarySearchFunc(int[] nums, int target)
        {
            int left = 0;
            int right = nums.Length - 1;
            while (left <= right)
            {
                int mid = (right + left) / 2;

                if (nums[mid] == target)
                {
                    Console.WriteLine($"Element found at index: {mid}");
                    return;
                }
                else if (nums[mid] < target)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
        }
    }
}
