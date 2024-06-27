using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity
{
    public static class ApplicationIdentityDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

                await SeedUserAsync(userManager, dbContext);
            }
        }

        private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, ApplicationIdentityDbContext dbContext)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<ApplicationUser>
            {
                new() {
                    UserName = "uyen.dinhluu@ips-ag.com",
                    Email = "uyen.dinhluu@ips-ag.com",
                    DisplayName = "Uyen Luu",
                    SoftwareDistributionGroup = "Stable",
                    Address = new Address
                    {
                        Id = Guid.NewGuid().ToString(),
                        Fname = "Uyen",
                        Lname = "Luu",
                        Street = "11 Le Dinh Ly",
                        City = "Danang",
                        State = "Jharkhand",
                        ZipCode = "123456"
                    }
                },
                new() {
                    UserName = "anh.quangtran@ips-ag.com",
                    Email = "anh.quangtran@ips-ag.com",
                    DisplayName = "Steve",
                    SoftwareDistributionGroup = "EarlyAccess",
                    Address = new Address
                    {
                        Id = Guid.NewGuid().ToString(),
                        Fname = "Steve",
                        Lname = "Steve",
                        Street = "11 Le Dinh Ly",
                        City = "Danang",
                        State = "Jharkhand",
                        ZipCode = "123456"
                    }
                }
            };

                foreach (var user in users)
                {
                    var result = await userManager.CreateAsync(user, "1!LeDinhLy");

                    if (result.Succeeded)
                    {
                        // Optionally, you can do additional seeding or customization here
                        // For example, add user roles, claims, etc.
                    }
                    else
                    {
                        throw new Exception($"User creation failed: {string.Join(", ", result.Errors)}");
                    }
                }
            }
        }
    }
}
