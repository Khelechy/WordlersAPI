using AutoMapper;
using WordlersAPI.Models.Core;
using WordlersAPI.Models.Dto;

namespace WordlersAPI.Mapper
{
    public class WordlerMapping : Profile
    {
        public WordlerMapping()
        {
            CreateMap<User, UserDto>();
        }
    }
}
