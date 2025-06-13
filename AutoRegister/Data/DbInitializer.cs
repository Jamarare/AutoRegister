using Microsoft.AspNetCore.Identity;

public static class DbInitializer
{
    public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "User" };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        var adminEmail = "admin@auto.ee";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            var user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin"
            };
            var result = await userManager.CreateAsync(user, "Admin123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
