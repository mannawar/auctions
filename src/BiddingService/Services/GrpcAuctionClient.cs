using AuctionService;
using BiddingService.Models;
using Grpc.Net.Client;

namespace BiddingService.Services;

public class GrpcAuctionClient
{
    private ILogger<GrpcAuctionClient> _logger;
    private IConfiguration _config;

    public GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
    {
        _config = config;
        _logger = logger;
    }

    public Auction GetAuction(string id)
    {
        _logger.LogInformation("Calling Grpc service");
        var channel = GrpcChannel.ForAddress(_config["GrpcAuction"]);
        var client = new GrpcAuction.GrpcAuctionClient(channel);
        var request = new GetAuctionRequest { Id = id };

        try
        {
            var reply = client.GetAuction(request);
            var auction = new Auction
            {
                ID = reply.Auction.Id,
                AuctionEnd = DateTime.Parse(reply.Auction.AuctionEnd),
                Seller = reply.Auction.Seller,
                ReservePrice = reply.Auction.ReservePrice
            };
            return auction;
        }
        catch (Exception e)
        {
            _logger.LogInformation(e, "Could not call Grpc Server");
            return null;
        }
    }
}