namespace APBD_CW_API2.Models.DTOs;

public class Client_Trip
{
    int IdClient { get; set; }
    int IdTrip { get; set; }
    int RegisteredAt { get; set; }
    int? PaymentDate { get; set; }
    
}