using WordlersAPI.Interfaces;
using Microsoft.AspNetCore.Hosting;
using WordlersAPI.Helpers;
using WordlersAPI.Models.Request;

namespace WordlersAPI.Services
{
    public class WordEngineService : IWordEngineService
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;
        public WordEngineService(IWebHostEnvironment webHostingEnvironment)
        {
            _webHostingEnvironment = webHostingEnvironment;
        }

        public string GenerateWord(int wordLength)
        {
            string word = WordManager.GenerateWord(wordLength, _webHostingEnvironment.ContentRootPath); 
            return word;    

        }

        public bool ValidateAnagram(CheckAnagramRequestModel checkAnagramRequestModel)
        {
            var isAnagram = AnagramChecker.AnagramCheck(checkAnagramRequestModel.OriginalWord, checkAnagramRequestModel.NewWord);
            return isAnagram; 
        }

        public bool ValidateWord(string word)
        {
            var isValid = WordManager.ValidateWord(word, _webHostingEnvironment.ContentRootPath);
            return isValid; 
        }
    }
}
