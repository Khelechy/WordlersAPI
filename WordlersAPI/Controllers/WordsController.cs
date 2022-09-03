using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WordlersAPI.Interfaces;
using WordlersAPI.Models.Request;
using WordlersAPI.Services;

namespace WordlersAPI.Controllers
{
    [Route("api/words")]
    [ApiController]
    public class WordsController : ControllerBase
    {
        private readonly IWordEngineService _wordEngineService;

        public WordsController(IWordEngineService wordEngineService)
        {
            _wordEngineService = wordEngineService;
        }

        [Route("generate")]
        [HttpGet]
        public IActionResult GenerateWord([FromQuery] int wordLength)
        {
            string word = _wordEngineService.GenerateWord(wordLength);  
            return Ok(word);
        }

        [Route("shuffle/generate")]
        [HttpGet]
        public IActionResult GenerateShuffledWord([FromQuery] int wordLength)
        {
            string word = _wordEngineService.GenerateShuffledWord(wordLength);
            return Ok(word);
        }

        [Route("validate")]
        [HttpGet]
        public IActionResult ValidateWord([FromQuery] string word)
        {
            bool isValid = _wordEngineService.ValidateWord(word);
            return Ok(isValid);
        }

        [Route("anagram")]
        [HttpPost]
        public IActionResult ValidateAnagram([FromBody] CheckAnagramRequestModel checkAnagramRequestModel)
        {
            bool isAnagram = _wordEngineService.ValidateAnagram(checkAnagramRequestModel);
            return Ok(isAnagram);
        }

    }



}
