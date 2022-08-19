using Newtonsoft.Json.Linq;

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
    }
}
