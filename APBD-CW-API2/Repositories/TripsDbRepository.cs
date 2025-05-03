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
    Task RemoveClientFromTripAsync(int idClient, int idTrip);
}

public class TripsDbRepository(IConfiguration config) : ITripsDbRepository
{
    private readonly string? _connectionString = config.GetConnectionString("Default");

    // Pobiera wszystkie wycieczki i powiązane kraje
    public async Task<IEnumerable<TripCountryGetDTO>> GetAllTripsAsync()
    {
        var result = new List<TripCountryGetDTO>();
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT 
                Trip.IdTrip,
                Trip.Name,
                Trip.Description,
                Trip.DateFrom,
                Trip.DateTo,
                Trip.MaxPeople,
                Country.Name
            FROM 
                Trip
            JOIN 
                Country_Trip ON Trip.IdTrip = Country_Trip.IdTrip
            JOIN 
                Country ON Country_Trip.IdCountry = Country.IdCountry;";

        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new TripCountryGetDTO
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

    // Pobiera wszystkie wycieczki przypisane do konkretnego klienta
    public async Task<IEnumerable<TripByClientIdGetDTO>> GetTripsByClientIdAsync(int id)
    {
        var result = new List<TripByClientIdGetDTO>();
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT
                Trip.IdTrip,
                Trip.Name,
                Trip.Description,
                Trip.DateFrom,
                Trip.DateTo,
                Trip.MaxPeople,
                Client_Trip.RegisteredAt,
                Client_Trip.PaymentDate
            FROM 
                Client
            JOIN 
                Client_Trip ON Client.IdClient = Client_Trip.IdClient
            JOIN 
                Trip ON Client_Trip.IdTrip = Trip.IdTrip
            WHERE 
                Client.IdClient = @id;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new TripByClientIdGetDTO
            {
                IdTrip = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                DateFrom = reader.GetDateTime(3),
                DateTo = reader.GetDateTime(4),
                MaxPeople = reader.GetInt32(5),
                RegisteredAt = reader.GetInt32(6),
                PaymentDate = reader.IsDBNull(7) ? null : reader.GetInt32(7)
            });
        }
        return result;
    }

    // Sprawdza, czy klient o danym ID istnieje
    public async Task<Client> GetClientById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT IdClient 
            FROM Client 
            WHERE IdClient = @id;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? new Client
        {
            IdClient = reader.GetInt32(0)
        } : null;
    }

    // Tworzy nowego klienta i zwraca obiekt z jego ID
    public async Task<Client> CreateClientAsync(ClientCreateDTO client)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) 
            VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel); 
            SELECT SCOPE_IDENTITY();";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FirstName", client.FirstName);
        command.Parameters.AddWithValue("@LastName", client.LastName);
        command.Parameters.AddWithValue("@Email", client.Email);
        command.Parameters.AddWithValue("@Telephone", client.Telephone);
        command.Parameters.AddWithValue("@Pesel", client.Pesel);

        await connection.OpenAsync();
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

    // Pobiera dane wycieczki na podstawie ID
    public async Task<Trip> GetTripById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = "SELECT * FROM Trip WHERE IdTrip = @id;";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
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

    // Zwraca liczbę klientów przypisanych do danej wycieczki
    public async Task<Int32> GetPersonCountByTripId(int id)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT COUNT(*) 
            FROM Client_Trip 
            WHERE IdTrip = @id;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? reader.GetInt32(0) : 0;
    }

    // Dodaje klienta do wycieczki (rejestruje go)
    public async Task AddClientToTripAsync(int idClient, int idTrip)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate)
            VALUES (
                @IdClient,
                @IdTrip,
                CAST(CONVERT(VARCHAR(8), GETDATE(), 112) AS INT),
                NULL);";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@IdClient", idClient);
        command.Parameters.AddWithValue("@IdTrip", idTrip);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    // Pobiera dane o przypisaniu klienta do wycieczki
    public async Task<Client_TripGetDTO> GetClient_TripAsync(int idClient, int idTrip)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT * 
            FROM Client_Trip 
            WHERE IdClient = @idClient AND IdTrip = @idTrip;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idClient", idClient);
        command.Parameters.AddWithValue("@idTrip", idTrip);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? new Client_TripGetDTO
        {
            IdClient = reader.GetInt32(0),
            IdTrip = reader.GetInt32(1),
            RegisteredAt = reader.GetInt32(2),
            PaymentDate = reader.IsDBNull(3) ? null : reader.GetInt32(3)
        } : null;
    }

    // Usuwa przypisanie klienta do wycieczki
    public async Task RemoveClientFromTripAsync(int idClient, int idTrip)
    {
        await using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            DELETE FROM Client_Trip 
            WHERE IdClient = @idClient AND IdTrip = @idTrip;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idClient", idClient);
        command.Parameters.AddWithValue("@idTrip", idTrip);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }
}
