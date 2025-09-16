using CodingJournal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CodingJournal.Domain.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var testEmail = "test@test.com";
        var testUser = await userManager.FindByEmailAsync(testEmail);
        if (testUser == null)
        {
            testUser = new User { 
                UserName = testEmail, 
                Email = testEmail, 
                FirstName = "Test", 
                LastName = "User", 
                EmailConfirmed = true 
            };
            var result = await userManager.CreateAsync(testUser, "test");
            
            if (result.Succeeded)
            {
                Console.WriteLine($"Test user created with ID: {testUser.Id}");
            }
            else
            {
                Console.WriteLine("❌ Failed to create test user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"   - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($"Test user already exists with ID: {testUser.Id}");
        }
    }
}