using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Questions
{
    public static class MaskWord
    {
        public static void CallMaskWord()
        {
            string res = ProcessSentence("Keep the first and last letter of the word");
            Console.WriteLine($"Result is {res}");
        }
        static string ProcessSentence(string input)
        {
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                int ptr = i;//starting index of a new word
                while (ptr < input.Length && input[ptr] != ' ') ptr++;
                string word = input.Substring(i, ptr - i);
                res.Append(ProcessWord(word));
                if (ptr < input.Length && input[ptr] == ' ')
                    res.Append(' ');
                i = ptr;
            }

            return res.ToString();
        }

        static string ProcessWord(string word)
        {
            if (word.Length <= 2) return word;
            string sub = word.Substring(1, word.Length - 2);
            word = word[0].ToString() + new string('*', sub.Length) + word[word.Length - 1].ToString();
            return word;
        }
    }
}
