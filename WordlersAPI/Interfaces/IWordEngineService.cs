using WordlersAPI.Models.Request;

namespace WordlersAPI.Interfaces
{
    public interface IWordEngineService
    {
        string GenerateWord(int wordLength);
        bool ValidateAnagram(CheckAnagramRequestModel checkAnagramRequestModel);
        bool ValidateWord(string word);
    }
}
