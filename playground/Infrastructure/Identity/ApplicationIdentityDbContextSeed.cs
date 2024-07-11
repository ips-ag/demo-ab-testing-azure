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
                var userByGroup = new Dictionary<string, string[]>
                {
                    {"EarlyAccess" , ["uyen.dinhluu@ips-ag.com", "uyen1.dinhluu@ips-ag.com", "uyen2.dinhluu@ips-ag.com"]},
                    {"Stable" , ["anh.quangtran@ips-ag.com", "anh1.quangtran@ips-ag.com"]}
                };

                var users = userByGroup.SelectMany(g => g.Value.Select((emailAddress, index) => new ApplicationUser
                {
                    UserName = emailAddress,
                    Email = emailAddress,
                    DisplayName = $"Uyen Luu{(index > 0 ? index : string.Empty)} - EarlyAccess",
                    SoftwareDistributionGroup = "EarlyAccess",
                    Address = new Address
                    {
                        Id = Guid.NewGuid().ToString(),
                        Fname = "Fname",
                        Lname = "Lname",
                        Street = "11 Le Dinh Ly",
                        City = "Danang",
                        State = "Thanh Khe",
                        ZipCode = "50000"
                    }
                }));

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
