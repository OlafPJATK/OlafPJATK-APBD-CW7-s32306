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
    Task AddClientToTripAsync(int id, int tripId);
    Task RemoveClientFromTripAsync(int id, int tripId);
    
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
        var client = await _tripsDbRepository.GetClientByIdAsync(id);
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

    public async Task<Client> CreateClientAsync(ClientCreateDTO client)
    {
        
        return await _tripsDbRepository.CreateClientAsync(client);
    }

    public async Task AddClientToTripAsync(int id, int tripId)
    {
        var client = await _tripsDbRepository.GetClientByIdAsync(id);
        if (client == null) throw new NotFoundException("Client does not exist.");
        var trip = await _tripsDbRepository.GetTripByIdAsync(tripId);
        if (trip == null) throw new NotFoundException("Trip does not exist.");
        var clientsInTrip = await _tripsDbRepository.GetPersonCountByTripIdAsync(tripId);  
        if (clientsInTrip >= trip.MaxPeople) throw new BadRequestException("Trip is full.");
        if (await _tripsDbRepository.GetClient_TripAsync(id,tripId) != null)
        {
           throw new BadRequestException("Client is already in trip.");
        }
        await _tripsDbRepository.AddClientToTripAsync(id,tripId);
    }

    public async Task RemoveClientFromTripAsync(int id, int tripId)
    {   
        var client = await _tripsDbRepository.GetClientByIdAsync(id);
        if (client == null) throw new NotFoundException("Client does not exist.");
        var trip = await _tripsDbRepository.GetTripByIdAsync(tripId);
        if (trip == null) throw new NotFoundException("Trip does not exist.");
        var clientTrp = await _tripsDbRepository.GetClient_TripAsync(id,tripId);
        if (clientTrp == null) throw new NotFoundException("Client is not in trip.");
       
        await _tripsDbRepository.RemoveClientFromTripAsync(id,tripId);
    }
}