using System;
using System.Collections.Generic;

namespace DSA_Questions
{
    public static class MoveZeroesToEnd
    {
        public static void callMoveZeroToEnd()
        {
            MoveZeroToEnd();
            //m(10, 20);
        }

        public static void BruteForce_MoveZeroToEnd()
        {
            int[] arr = { 2, 0, 0, 0, 0, 1, 0, 3, 12 };  // Output: [2, 1, 3, 12, 0, 0]

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == 0)
                {
                    for (int j = i + 1; j < arr.Length; j++)
                    {
                        if (arr[j] != 0)
                        {
                            int temp = arr[i];
                            arr[i] = arr[j];
                            arr[j] = temp;
                        }
                        else j++;
                    }
                }
            }

            Console.WriteLine("Using BruteForce Approach:");
            foreach (int num in arr)
            {
                Console.Write(num + " ");
            }
        }

        public static void MoveZeroToEnd()
        {
            int[] nums = { 23, 0, 0, 0, 0, 1, 0, 3, 12 };

            int lastnz = 0;

            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] != 0)
                {
                    (nums[lastnz], nums[i]) = (nums[i], nums[lastnz]);
                    lastnz++;
                }
            }
            for (int j = lastnz; j < nums.Length; j++)
            {
                nums[j] = 0;
            }

            Console.WriteLine("Using Optimized Approach:");
            foreach (int num in nums)
            {
                Console.Write(num + " ");
            }

        }

        public static int Sum(int a, int b)
        {
            var sum = a + b;

            // log to the file
            File.AppendAllText("logSum.txt", $"Sum of {a} and {b} is {sum}{Environment.NewLine}");

            return sum;
        }
    }
}
