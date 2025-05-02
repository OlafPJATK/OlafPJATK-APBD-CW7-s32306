using APBD_CW_API2.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_CW_API2.Services;

public interface IDbService
{
    Task<IEnumerable<TripPlusCountryInfoGetDTO>> GetClientTripsByClientIdAsync(int id);
    Task<IEnumerable<TripPlusCountryInfoGetDTO>> GetAllTripsAsync();
}

public class DbService(IConfiguration config) : IDbService
{
    private readonly string? _connectionString=config.GetConnectionString("Default");


    public async Task<IEnumerable<TripPlusCountryInfoGetDTO>> GetClientTripsByClientIdAsync(int id)
    {
        var result = new List<TripPlusCountryInfoGetDTO>();
        
        await using var connection = new SqlConnection(_connectionString);
        
        const string sql = "SELECT T.*\nFROM Client_Trip CT\nJOIN Trip T ON CT.IdTrip = T.IdTrip\n WHERE CT.IdClient = 1;\n";
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(
                new TripPlusCountryInfoGetDTO
                {
                    IdTrip = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    DateFrom = reader.GetDateTime(3),
                    DateTo = reader.GetDateTime(4),
                    MaxPeople = reader.GetInt32(5)
                });
        }
        return result;
    }
    
    public async Task<IEnumerable<TripPlusCountryInfoGetDTO>> GetAllTripsAsync()
    {
        var result = new List<TripPlusCountryInfoGetDTO>();
        
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
                new TripPlusCountryInfoGetDTO
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