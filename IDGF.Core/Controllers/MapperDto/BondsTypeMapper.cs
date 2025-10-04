using AutoMapper;

namespace IDGF.Core.Controllers.MapperDto
{
    public class BondsTypeMapper : Profile
    {
        public BondsTypeMapper()
        {
            CreateMap<Domain.BondsType, BondsTypeGetDto>()
                .ForMember(dest => dest.HasCoupon,
                    opt => opt.MapFrom(src => src.HasCoupon ? "دارد" : "ندارد"));
        }
    }
}
