namespace AuctionService.Entities;
using System.ComponentModel.DataAnnotations.Schema;


[Table("Item")]
public class Item {
    public Guid Id { get; set; }    
    public string Make { get; set; }    
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public int Mileage { get; set; }
    public string ImageUrl { get; set; }

    //Defining nav property by convention to relate with Auction, we can also define by configuration.
    public Auction Auction { get; set; }
    public Guid AuctionId { get; set; }
}