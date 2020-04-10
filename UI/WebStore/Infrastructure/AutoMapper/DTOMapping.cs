using AutoMapper;
using WebStore.Domain.DTO.Products;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;

namespace WebStore.Infrastructure.AutoMapper
{
    public class DTOMapping : Profile
    {
        public DTOMapping()
        {
            CreateMap<ProductDTO, ProductViewModel>().ReverseMap();
            CreateMap<ProductDTO, Product>().ReverseMap();
        }
    }
}
