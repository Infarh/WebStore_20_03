using AutoMapper;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.ViewModels.Identity;

namespace WebStore.Infrastructure.AutoMapper
{
    public class ViewModelsMapping : Profile
    {
        public ViewModelsMapping()
        {
            CreateMap<RegisterUserViewModel, User>()
               .ForMember(user => user.UserName, opt => opt.MapFrom(model => model.UserName));
        }
    }
}
