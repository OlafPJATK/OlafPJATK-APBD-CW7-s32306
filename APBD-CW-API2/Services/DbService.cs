using APBD_CW_API2.Exceptions;
using APBD_CW_API2.Models;
using APBD_CW_API2.Models.DTOs;
using APBD_CW_API2.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace APBD_CW_API2.Services;

public interface IDbService
{
    Task<IEnumerable<TripByClientIdGetDTO>> GetTripsByClientIdAsync(int id);
    Task<IEnumerable<TripCountryGetDTO>> GetAllTripsAsync();
    Task<Client> CreateClientAsync(ClientCreateDTO client);
}

public class DbService : IDbService
{
   private readonly ITripsDbRepository _tripsDbRepository;

   public DbService(ITripsDbRepository tripsDbRepository)
   {
       _tripsDbRepository = tripsDbRepository;
   }

   public async Task<IEnumerable<TripByClientIdGetDTO>> GetTripsByClientIdAsync(int id)
    {
        var client = await _tripsDbRepository.GetClientById(id);
        if (client == null) throw new NotFoundException("Client does not exist.");
        
        var trips = await _tripsDbRepository.GetTripsByClientIdAsync(id);
        if (trips.IsNullOrEmpty())
        {
            throw new NotFoundException("Client has no trips.");
        }
        return trips;
    }
    
    public async Task<IEnumerable<TripCountryGetDTO>> GetAllTripsAsync()
    {
        return await _tripsDbRepository.GetAllTripsAsync();
    }

    public Task<Client> CreateClientAsync(ClientCreateDTO client)
    {
        return _tripsDbRepository.CreateClientAsync(client);
    }
}