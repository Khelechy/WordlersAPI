using Newtonsoft.Json.Linq;
using System;

namespace WordlersAPI.Helpers
{
    public static class WordManager
    {
        public static string GenerateWord(int wordLength, string contentRootPath)
        {
            string dictionaryFile = File.ReadAllText(contentRootPath + $"Dictionary/{wordLength}letterword.json");
            JArray wordArray = JArray.Parse(dictionaryFile);
            Random r = new Random();
            int RandomWordIndex = r.Next(0, wordArray.Count);
            string word = (string)wordArray[RandomWordIndex];
            return word;

        }

        public static bool ValidateWord(string word, string contentRootPath)
        {
            int wordLength = word.Length;   
            string dictionaryFile = File.ReadAllText(contentRootPath + $"Dictionary/{wordLength}letterword.json");
            JArray wordArray = JArray.Parse(dictionaryFile);
            bool isValid = wordArray.Any(x => x.Value<string>() == word);
            return isValid;
        }


        public static string Shuffle(this string input){
            Random r = new Random();
            return new string(input.ToCharArray().OrderBy(s => (r.Next(2) % 2) == 0).ToArray());   
        }
    }
}
