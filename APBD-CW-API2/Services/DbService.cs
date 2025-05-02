using APBD_CW_API2.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_CW_API2.Services;

public interface IDbService
{
    Task<IEnumerable<TripByClientIdGetDTO>> GetTripsByClientIdAsync(int id);
    Task<IEnumerable<TripCountryGetDTO>> GetAllTripsAsync();
}

public class DbService(IConfiguration config) : IDbService
{
    private readonly string? _connectionString=config.GetConnectionString("Default");
    
    public async Task<IEnumerable<TripByClientIdGetDTO>> GetTripsByClientIdAsync(int id)
    {
        var result = new List<TripByClientIdGetDTO>();
        
        await using var connection = new SqlConnection(_connectionString);
        
        const string sql = "SELECT\n\n\tTrip.IdTrip,\n    Trip.Name,\n    Trip.Description,\n  " +
                           "  Trip.DateFrom,\n    Trip.DateTo,\n  " +
                           "  Trip.MaxPeople,\n    Client_Trip.RegisteredAt,\n  " +
                           "  Client_Trip.PaymentDate\nFROM \n    Client\nJOIN \n  " +
                           "  Client_Trip ON Client.IdClient = Client_Trip.IdClient\nJOIN \n  " +
                           "  Trip ON Client_Trip.IdTrip = Trip.IdTrip\nWHERE \n    Client.IdClient = @id;\n";
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            
            result.Add(
                new TripByClientIdGetDTO
                {
                    IdTrip = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    RegisteredAt = reader.GetInt32(6),
                    PaymentDate =reader.IsDBNull(7) ? null : reader.GetInt32(7)
                });
        }
        return result;
    }
    
    public async Task<IEnumerable<TripCountryGetDTO>> GetAllTripsAsync()
    {
        var result = new List<TripCountryGetDTO>();
        
        await using var connection = new SqlConnection(_connectionString);

        const string sql = "\nSELECT \n\tTrip.IdTrip,\n    Trip.Name,\n    Trip.Description,\n    Trip.DateFrom,\n" +
                           "    Trip.DateTo,\n    Trip.MaxPeople,\n    Country.Name\nFROM \n    Trip\nJOIN \n" +
                           "    Country_Trip ON Trip.IdTrip = Country_Trip.IdTrip\nJOIN \n" +
                           "    Country ON Country_Trip.IdCountry = Country.IdCountry;\n";
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(
                new TripCountryGetDTO
                {
                    IdTrip = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5),
                    CountryName = reader.GetString(6)
                });
        }
        return result;
    }
}