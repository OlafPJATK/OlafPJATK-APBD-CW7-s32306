using System.ComponentModel.DataAnnotations;

namespace APBD_CW_API2.Models.DTOs;

public class ClientCreateDTO
{
    public int IdClient { get; set; }
    [Required]
    [Length(1, 120)]
    public string FirstName { get; set; }
    [Required]
    [Length(1, 120)]
    public string LastName { get; set; }
    [Required]
    [Length(1, 120)]
    public string Email { get; set; }
    [Required]
    [Length(1, 120)]
    public string Telephone { get; set; }
    [Required]
    [Length(1, 120)]
    public string Pesel { get; set; }
    
}