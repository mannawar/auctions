using AutoMapper;
using Contracts;

namespace SearchService.RequestsHelper;

public class MappingProfiles: Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();

    }
}