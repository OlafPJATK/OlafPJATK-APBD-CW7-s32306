using APBD_CW_API2.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_CW_API2.Services;

public interface IDbService
{
    Task<IEnumerable<ClientGetDTO>> GetMyClientsAsync();
}

public class DbService(IConfiguration config) : IDbService
{
    private readonly string? _connectionString=config.GetConnectionString("Default");


    public async Task<IEnumerable<ClientGetDTO>> GetMyClientsAsync()
    {
        var result = new List<ClientGetDTO>();
        
        await using var connection = new SqlConnection(_connectionString);
        const string sql = "SELECT * FROM Client";
        await using var command = new SqlCommand(sql, connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(
                new ClientGetDTO
                {
                    IdClient = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3),
                    Telephone = reader.GetString(4),
                    Pesel = reader.GetString(5),
                });
        }
        return result;
    }
}