namespace WordlersAPI.Helpers
{
    public static class AnagramChecker
    {
        public static bool AnagramCheck(string a, string b)
        {
            List<char> list = a.ToLower().ToList();
            bool isAnagram = b.All(ch => list.Remove(ch));
            return isAnagram;
        }
    }
}
