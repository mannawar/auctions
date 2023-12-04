using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.DTOS;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController: ControllerBase {

    private readonly IAuctionRepository _repo;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishedEndpoint;

    public AuctionsController(IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishedEndpoint)

    {
        _repo = repo;
        _mapper = mapper;
        _publishedEndpoint = publishedEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>>GetAllAuctions(string date)
    {
        return await _repo.GetAuctionsAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id){
        var auction = await _repo.GetAuctionByIdAsync(id);
        
        if(auction == null) return NotFound();
        return auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto){

        var auction = _mapper.Map<Auction>(auctionDto);
        auction.Seller = User.Identity.Name;

        _repo.AddAuction(auction);

        var newAuction = _mapper.Map<AuctionDto>(auction);

        await _publishedEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _repo.SaveChangesAsync();

        if(!result) return BadRequest("Could not save changes to db");
        return CreatedAtAction(nameof(GetAuctionById),new {auction.Id}, newAuction);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdatedAuctionDto updatedAuctionDto){
        var auction = await _repo.GetAuctionEntityById(id);
        
        if(auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();
        auction.Item.Make = updatedAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updatedAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Year = updatedAuctionDto.Year ?? auction.Item.Year;
        auction.Item.Color = updatedAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updatedAuctionDto.Mileage ?? auction.Item.Mileage;

        await _publishedEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _repo.SaveChangesAsync();
        if(result) return Ok();

        return BadRequest("Problem updating records");

    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id){
        var auction = await _repo.GetAuctionEntityById(id);

        if(auction == null) return NotFound();
        if (auction.Seller != User.Identity.Name) return Forbid();

        _repo.RemoveAuction(auction);

        //todo
        await _publishedEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result = await _repo.SaveChangesAsync();

        if(!result) return BadRequest("Could not delete auction");
        return Ok();
    }
}