using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.DTOS;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController: ControllerBase {
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishedEndpoint;

    public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishedEndpoint)

    {
        _context = context;
        _mapper = mapper;
        _publishedEndpoint = publishedEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>>GetAllAuctions(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }


        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id){
        var auction = await _context.Auctions
                        .Include(x => x.Item)
                        .FirstOrDefaultAsync(x => x.Id == id);
        
        if(auction == null) return NotFound();
        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto){

        var auction = _mapper.Map<Auction>(auctionDto);
        auction.Seller = "test";

        _context.Auctions.Add(auction);

        var newAuction = _mapper.Map<AuctionDto>(auction);

        await _publishedEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save changes to db");
        return CreatedAtAction(nameof(GetAuctionById),new {auction.Id}, newAuction);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdatedAuctionDto updatedAuctionDto){
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if(auction == null) return NotFound();

        auction.Item.Make = updatedAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updatedAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Year = updatedAuctionDto.Year ?? auction.Item.Year;
        auction.Item.Color = updatedAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updatedAuctionDto.Mileage ?? auction.Item.Mileage;

        await _publishedEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _context.SaveChangesAsync() > 0;
        if(result) return Ok();

        return BadRequest("Problem updating records");

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id){
        var auction = await _context.Auctions.FindAsync(id);

        if(auction == null) return NotFound();

        _context.Auctions.Remove(auction);

        //todo
        await _publishedEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not delete auction");
        return Ok();
    }
}