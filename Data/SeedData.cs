using Data.Context;
using Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var dbContext = serviceProvider.GetRequiredService<EmployeeAppDbContext>();
        var moduleKeys = new[]
        {
            "personal_details",
            "qualifications",
            "next_of_kin",
            "hr_info",
            "documents"
        };

        var existingModules = await dbContext.OnboardingModules
            .Where(m => moduleKeys.Contains(m.Key))
            .ToListAsync();

        if (existingModules.Count != moduleKeys.Length)
        {
            var modules = new List<OnboardingModule>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Key = "personal_details",
                    Name = "Personal Details",
                    IsRequired = true,
                    SortOrder = 1,
                    Weight = 1
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Key = "qualifications",
                    Name = "Qualifications",
                    IsRequired = true,
                    SortOrder = 2,
                    Weight = 1
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Key = "next_of_kin",
                    Name = "Next of Kin",
                    IsRequired = true,
                    SortOrder = 3,
                    Weight = 1
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Key = "hr_info",
                    Name = "HR Information",
                    IsRequired = true,
                    SortOrder = 4,
                    Weight = 1
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Key = "documents",
                    Name = "Documents",
                    IsRequired = true,
                    SortOrder = 5,
                    Weight = 1
                }
            };

            var missing = modules.Where(m => existingModules.All(e => e.Key != m.Key)).ToList();
            if (missing.Count > 0)
            {
                dbContext.OnboardingModules.AddRange(missing);
                await dbContext.SaveChangesAsync();
            }
        }

        // Create initial Admin user only when an explicit password is provided
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_INITIAL_PASSWORD");
        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var adminEmail = "admin@staffhub.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createPowerUser = await userManager.CreateAsync(adminUser, adminPassword);
            if (createPowerUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
