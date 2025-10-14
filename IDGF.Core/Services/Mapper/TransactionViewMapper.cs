using AutoMapper;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Infrastructure;

namespace IDGF.Core.Services.Mapper
{
    public class TransactionViewMapper : Profile
    {
        public TransactionViewMapper()
        {
            CreateMap<Domain.Views.TransactionBasicView, TransactionResult>()
                      .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Id))
            // Optional: you can rename or ignore members if needed
            //.ForMember(dest => dest.InvestmentAmount, opt => opt.Ignore())
            .ForMember(dest => dest.MaturityAmount, opt => opt.Ignore())
            .ForMember(dest => dest.SimpleYield, opt => opt.Ignore())
            .ForMember(dest => dest.YieldToMaturity, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPurchase, opt => opt.Ignore())
            .ForMember(dest => dest.StatusText, opt => opt.Ignore());
        }
    }
}
