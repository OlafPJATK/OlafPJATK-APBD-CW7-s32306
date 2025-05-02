namespace APBD_CW_API2.Models.DTOs;

public class Client_TripGetDTO
{
    public int IdClient { get; set; }
    public int IdTrip { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}