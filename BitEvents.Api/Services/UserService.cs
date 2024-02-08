using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Contracts.Requests;
using BitEvents.Api.Contracts.Responses;
using BitEvents.Api.Extensions;
using BitEvents.Api.Models;
using BitEvents.Api.Exceptions;
using BitEvents.Api.Mappers;
using BitEvents.Api.Repositories.Interfaces;
using BitEvents.Api.Services.Interfaces;

namespace BitEvents.Api.Services;

public sealed class UserService : IUserService
{
    private readonly IEventRepository _eventRepository;
    private readonly string _secret;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository,
        IConfiguration configuration,
        IEventRepository eventRepository)
    {
        _userRepository = userRepository;
        _secret = configuration["Authorization:Secret"]!;
        _eventRepository = eventRepository;
    }

    public async Task<List<User>> GetAllUsersAsync(UserQueryFilter userQueryFilter)
    {
        return await _userRepository.GetAllUsersAsync(userQueryFilter);
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<User> CreateUserAsync(UserCreateRequest userCreateRequest)
    {
        var existingUser = await _userRepository.GetUserWithEmailAsync(userCreateRequest.Email);
        if (existingUser is not null)
        {
            throw new InvalidInputException("User with given email already exists");
        }

        var user = UserMapper.UserCreateRequestToUser(userCreateRequest);
        (user.PasswordSalt, user.PasswordHash) = AuthHelpers.HashPassword(userCreateRequest.Password);

        user.Roles.Add("User");

        var success = await _userRepository.CreateUserAsync(user);
        if (!success)
        {
            throw new DatabaseException("Failed to create user");
        }

        return user;
    }

    public async Task<User?> UpdateUserAsync(string id, UserUpdateRequest userUpdateRequest)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser is null)
        {
            return null;
        }

        existingUser.FirstName = userUpdateRequest.FirstName;
        existingUser.LastName = userUpdateRequest.LastName;
        existingUser.UpdatedAtUtc = DateTime.UtcNow;
        var success = await _userRepository.UpdateUserAsync(existingUser);
        if (!success)
        {
            throw new DatabaseException("Failed to update the user");
        }

        return existingUser;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser is null)
        {
            return false;
        }

        existingUser.IsDeleted = true;
        existingUser.DeletedAtUtc = DateTime.UtcNow;
        return await _userRepository.UpdateUserAsync(existingUser);
    }

    public async Task<TokenResponse> AuthenticateAsync(AuthenticateRequest authenticateRequest)
    {
        var user = await _userRepository.GetUserWithEmailAsync(authenticateRequest.Email);

        if (user is null)
        {
            throw new InvalidInputException("Incorrect email");
        }

        if (!user.ValidatePassword(authenticateRequest.Password))
        {
            throw new InvalidInputException("Incorrect password");
        }

        var (token, refreshToken) = user.GenerateTokens(_secret);
        user.RefreshTokens.Add(new RefreshToken { Token = refreshToken, ExpireDate = DateTime.UtcNow.AddDays(7) });
        await _userRepository.UpdateUserAsync(user);
        return new TokenResponse { Token = token, RefreshToken = refreshToken };
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var user = await _userRepository.GetUserWithRefreshTokenAsync(refreshTokenRequest.Token);

        if (user is null)
        {
            throw new InvalidInputException("Invalid token");
        }

        var oldToken = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshTokenRequest.Token)!;
        if (oldToken.ExpireDate < DateTime.UtcNow)
        {
            throw new InvalidInputException("Token expired");
        }

        var (token, refreshToken) = user.GenerateTokens(_secret);
        user.RefreshTokens.Remove(oldToken);
        user.RefreshTokens.Add(new RefreshToken { Token = refreshToken, ExpireDate = DateTime.UtcNow.AddDays(7) });
        await _userRepository.UpdateUserAsync(user);
        return new TokenResponse { Token = token, RefreshToken = refreshToken };
    }
    
    public async Task<bool> AddEventToFavouritesAsync(string userId,
        string eventId,
        CancellationToken ct)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException($"There is no user with the id of {userId}");
        }

        var eventFromDb = await _eventRepository.GetEventByIdAsync(eventId);
        if (eventFromDb is null)
        {
            throw new NotFoundException($"There is no event with the id of {eventId}");
        }
        
        if(user.FavouriteEvents.Contains(EventMapper.EventToEventPartial(eventFromDb)))
        {
            throw new InvalidInputException("You are already going to this event");
        }

        user.FavouriteEvents.Add(EventMapper.EventToEventPartial(eventFromDb));
        eventFromDb.Favourites++;
        var updateEventTask = _eventRepository.UpdateEventAsync(eventFromDb);
        var updateUserTask = _userRepository.UpdateUserAsync(user);

        Task.WaitAll(new Task[] { updateEventTask, updateUserTask }, cancellationToken: ct);

        return true;
    }

    public async Task<bool> RemoveEventFromFavouritesAsync(string userId,
        string eventId,
        CancellationToken ct)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException($"There is no user with the id of {userId}");
        }

        var eventFromDb = await _eventRepository.GetEventByIdAsync(eventId);
        if (eventFromDb is null)
        {
            throw new NotFoundException($"There is no event with the id of {eventId}");
        }
        
        if(user.GoingEvents.Contains(EventMapper.EventToEventPartial(eventFromDb)))
        {
            throw new InvalidInputException("You are already going to this event");
        }

        user.GoingEvents.Add(EventMapper.EventToEventPartial(eventFromDb));
        eventFromDb.Going++;
        var updateEventTask = _eventRepository.UpdateEventAsync(eventFromDb);
        var updateUserTask = _userRepository.UpdateUserAsync(user);

        Task.WaitAll(new Task[] { updateEventTask, updateUserTask }, cancellationToken: ct);

        return true;
    }

    public async Task<bool> AddEventToGoingAsync(string userId,
        string eventId,
        CancellationToken ct)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException($"There is no user with the id of {userId}");
        }

        var eventFromDb = await _eventRepository.GetEventByIdAsync(eventId);
        if (eventFromDb is null)
        {
            throw new NotFoundException($"There is no event with the id of {eventId}");
        }
        
        if(!user.FavouriteEvents.Contains(EventMapper.EventToEventPartial(eventFromDb)))
        {
            throw new InvalidInputException("You are already going to this event");
        }

        user.FavouriteEvents.Remove(EventMapper.EventToEventPartial(eventFromDb));
        eventFromDb.Favourites--;
        var updateEventTask = _eventRepository.UpdateEventAsync(eventFromDb);
        var updateUserTask = _userRepository.UpdateUserAsync(user);

        Task.WaitAll(new Task[] { updateEventTask, updateUserTask }, cancellationToken: ct);

        return true;
    }

    public async Task<bool> RemoveEventFromGoingAsync(string userId,
        string eventId,
        CancellationToken ct)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException($"There is no user with the id of {userId}");
        }

        var eventFromDb = await _eventRepository.GetEventByIdAsync(eventId);
        if (eventFromDb is null)
        {
            throw new NotFoundException($"There is no event with the id of {eventId}");
        }

        if(!user.GoingEvents.Contains(EventMapper.EventToEventPartial(eventFromDb)))
        {
            throw new InvalidInputException("You are already going to this event");
        }

        user.FavouriteEvents.Remove(EventMapper.EventToEventPartial(eventFromDb));
        eventFromDb.Favourites--;
        var updateEventTask = _eventRepository.UpdateEventAsync(eventFromDb);
        var updateUserTask = _userRepository.UpdateUserAsync(user);

        Task.WaitAll(new Task[] { updateEventTask, updateUserTask }, cancellationToken: ct);

        return true;
    }
}