using AutoMapper;
using LibraryCMS.API.DTOs;
using LibraryCMS.API.Models;

namespace LibraryCMS.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Author, AuthorDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();

            CreateMap<CreateBookDTO, Book>();
            CreateMap<CreateAuthorDTO, Author>();
            CreateMap<CreateCategoryDTO, Category>();
        }
    }
}
