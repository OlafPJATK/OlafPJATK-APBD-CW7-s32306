using APBD_CW_API2.Exceptions;
using APBD_CW_API2.Models;
using APBD_CW_API2.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_CW_API2.Repositories;

public interface ITripsDbRepository
{
    Task<IEnumerable<TripCountryGetDTO>> GetAllTripsAsync(); 
    Task<IEnumerable<TripByClientIdGetDTO>> GetTripsByClientIdAsync(int id);
    Task<Client> GetClientById(int id);
    Task<Client> CreateClientAsync(ClientCreateDTO client);
    Task<Trip> GetTripById(int id);
    Task<Int32> GetPersonCountByTripId(int id);
    Task AddClientToTripAsync(int idClient, int idTrip);
    Task<Client_TripGetDTO> GetClient_TripAsync(int idClient, int idTrip);
}

public class TripsDbRepository(IConfiguration config) : ITripsDbRepository
{
    private readonly string? _connectionString=config.GetConnectionString("Default");
    
    
    
    
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
    public async Task<Client> GetClientById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = "SELECT \n\tClient.IdClient\nFROM \n\tClient\nWHERE \n\tClient.IdClient = @id;";
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? new Client
        {
            IdClient = reader.GetInt32(0),
           
        } : null;
    }
    public async Task<Client> CreateClientAsync(ClientCreateDTO client)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "insert into Client (FirstName, LastName, Email, Telephone,Pesel) values (@FirstName, @LastName, @Email, @Telephone,@Pesel); Select scope_identity()";
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FirstName", client.FirstName);
        command.Parameters.AddWithValue("@LastName", client.LastName);
        command.Parameters.AddWithValue("@Email", client.Email);
        command.Parameters.AddWithValue("@Telephone", client.Telephone);
        command.Parameters.AddWithValue("@Pesel", client.Pesel);
        var id = Convert.ToInt32(await command.ExecuteScalarAsync());

        return new Client
        {

            IdClient = id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Telephone = client.Telephone,
            Pesel = client.Pesel

        };
    }
    public async Task<Trip> GetTripById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "select * from Trip where IdTrip=@id";
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? new Trip
        {
            IdTrip = reader.GetInt32(0),
            Name = reader.GetString(1),
            Description = reader.GetString(2),
            DateFrom = reader.GetDateTime(3),
            DateTo = reader.GetDateTime(4),
            MaxPeople = reader.GetInt32(5)
        } : null;
    }

    public async Task<Int32> GetPersonCountByTripId(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT COUNT(*) AS LiczbaUczestnikow\nFROM Client_Trip\nWHERE IdTrip = 5;";
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        command.Parameters.AddWithValue("@id", id);
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? reader.GetInt32(0) : 0;
    }

    public async Task AddClientToTripAsync(int idClient, int idTrip)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)\nVALUES (\n   "
                           +" @IdClient,\n    @IdTrip,\n  " 
        +"CAST(CONVERT(VARCHAR(8), GETDATE(), 112) AS INT),\n    NULL\n);\n";
        await using var command = new SqlCommand(sql, connection);
        
        await connection.OpenAsync();
        command.Parameters.AddWithValue("@IdClient", idClient);
        command.Parameters.AddWithValue("@IdTrip", idTrip);
        await command.ExecuteNonQueryAsync();
    }
    public async Task<Client_TripGetDTO> GetClient_TripAsync(int idClient, int idTrip)
    {
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT *\nFROM Client_Trip\nWHERE IdClient = @idClient AND IdTrip = @idTrip;\n";
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        command.Parameters.AddWithValue("@idClient", idClient);
        command.Parameters.AddWithValue("@idTrip", idTrip);
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? 
           new Client_TripGetDTO
           {
               IdClient = reader.GetInt32(0),
               IdTrip = reader.GetInt32(1),
               RegisteredAt = reader.GetInt32(2),
               PaymentDate = reader.IsDBNull(3) ? null : reader.GetInt32(3)
           }:null;
    }
}