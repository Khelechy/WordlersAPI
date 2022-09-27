using WordlersAPI.Models.Constants;

namespace WordlersAPI.Helpers
{
    public static class PointGrader
    {
        public static int AwardPoint(string word)
        {
            return word.Length + Constant.BufferPoint;
        }
    }
}
