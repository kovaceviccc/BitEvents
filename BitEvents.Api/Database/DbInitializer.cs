using BitEvents.Api.Contracts.QueryFilters;
using BitEvents.Api.Extensions;
using BitEvents.Api.Models;
using BitEvents.Api.Repositories.Interfaces;

namespace BitEvents.Api.Database;

public static class DbInitializer
{
    public static async Task InitializeAsync(IUserRepository userRepository,
        ILocationRepository locationRepository,
        ICategoryRepository categoryRepository)
    {
        var tmp = (await userRepository.GetAllUsersAsync(new UserQueryFilter())).Any();   
        
        if (!tmp)
        {
            var admin = new User
            {
                FirstName = "Admin",
                LastName = "Admin",
                Email = "admin@test.rs",
                Roles = new List<string>
                {
                    "Admin"
                },
                PasswordHash = "null",
                PasswordSalt = "null"
            };
            (admin.PasswordSalt, admin.PasswordHash) = AuthHelpers.HashPassword("Sifra.1234");
            await userRepository.CreateUserAsync(admin);

            var organization = new User
            {
                FirstName = "Organization",
                LastName = "Organization",
                Email = "org@test.rs",
                Roles = new List<string> { "Organization" },
                PasswordHash = "null",
                PasswordSalt = "null",
            };
            (organization.PasswordSalt, organization.PasswordHash) = AuthHelpers.HashPassword("Sifra.1234");
            await userRepository.CreateUserAsync(organization);

            var user = new User
            {
                FirstName = "User",
                LastName = "User",
                Email = "user@test.rs",
                Roles = new List<string> { "User" },
                RefreshTokens = new List<RefreshToken>
                {
                    new() { ExpireDate = DateTime.UtcNow.AddDays(-1), Token = "eaeaea" },
                    new() { ExpireDate = DateTime.UtcNow.AddMinutes(1), Token = "eaeaea2" }
                },
                PasswordHash = "null",
                PasswordSalt = "null"
            };
            (user.PasswordSalt, user.PasswordHash) = AuthHelpers.HashPassword("Sifra.1234");
            await userRepository.CreateUserAsync(user);
        }
        

        if (!(await locationRepository.GetAllLocationsAsync(new LocationQueryFilter())).Any())
        {

            var location = new Location
            {
                Name = "Lokacija"
            };
            await locationRepository.CreateLocationAsync(location);
        }

        if (!(await categoryRepository.GetAllCategoriesAsync(new CategoryQueryFilter())).Any())
        {
            var category = new Category
            {
                Name = "Kategorija"
            };
            await categoryRepository.CreateCategoryAsync(category);
        }
    }
}