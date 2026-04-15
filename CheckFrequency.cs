namespace DSA_Questions
{
    public static class CheckFrequency
    {
        public static void CallCheckFrequency()
        {
            FindElementFrequency();
        }

        public static void FindElementFrequency()
        {
            int[] arr = { 1, 2, 2, 3, 3, 3, 4 };

            Dictionary<int, int> frequencyDict = new Dictionary<int, int>();

            foreach (int num in arr)
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

            foreach (var elem in frequencyDict)
            {
                Console.WriteLine($"Element: {elem.Key}, Frequency: {elem.Value}");
            }

        }
    }
}
