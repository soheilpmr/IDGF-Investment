using AutoMapper;
using IDGF.Core.Controllers.Dtos;

namespace IDGF.Core.Controllers.MapperDto
{
    public class BrokerageMapper : Profile
    {

        public BrokerageMapper()
        {
            CreateMap<BrokerageCreateDto, Domain.Brokerage>().ReverseMap();
            CreateMap<BrokerageUpdateDto, Domain.Brokerage>().ReverseMap();
        }
    }
}
