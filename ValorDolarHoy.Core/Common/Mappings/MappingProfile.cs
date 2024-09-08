using AutoMapper;
using ValorDolarHoy.Core.Clients.Currency;
using ValorDolarHoy.Core.Services.Currency;

#pragma warning disable CS1591

namespace ValorDolarHoy.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        this.CreateMap<OficialResponse, OficialDto>()
            .ForMember(destinationMember => destinationMember.Buy,
                memberOptions => memberOptions.MapFrom(mapExpression => mapExpression.ValueBuy))
            .ForMember(destinationMember => destinationMember.Sell,
                memberOptions => memberOptions.MapFrom(mapExpression => mapExpression.ValueSell));

        this.CreateMap<BlueResponse, BlueDto>()
            .ForMember(destinationMember => destinationMember.Buy,
                memberOptions => memberOptions.MapFrom(mapExpression => mapExpression.ValueBuy))
            .ForMember(destinationMember => destinationMember.Sell,
                memberOptions => memberOptions.MapFrom(mapExpression => mapExpression.ValueSell));

        this.CreateMap<CurrencyResponse, CurrencyDto>()
            .ForMember(destinationMember => destinationMember.Official,
                memberOptions => memberOptions.MapFrom(mapExpression => mapExpression.Oficial))
            .ForMember(destinationMember => destinationMember.Blue,
                memberOptions => memberOptions.MapFrom(mapExpression => mapExpression.Blue));
    }
}
