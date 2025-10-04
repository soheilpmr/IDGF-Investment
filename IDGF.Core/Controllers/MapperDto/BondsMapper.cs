using AutoMapper;
using IDGF.Core.Controllers.Dtos;

namespace IDGF.Core.Controllers.MapperDto
{
    public class BondsMapper : Profile
    {
        public BondsMapper()
        {
            CreateMap<Domain.Bonds, BondsGetDto>().ReverseMap();
        }
    }
}
