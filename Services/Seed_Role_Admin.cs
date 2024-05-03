using Computer_Store.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Computer_Store.Services
{
	public static class Seed_Role_Admin
	{
		public static async Task InitializeAsync(IServiceProvider serviceProvider)
		{
			using (var scope = serviceProvider.CreateScope())
			{
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
				var userManager = scope.ServiceProvider.GetRequiredService<UserManager<SUser>>();

				await CreateRolesAndAdminUser(roleManager, userManager);
			}
		}

		private static async Task CreateRolesAndAdminUser(RoleManager<IdentityRole> roleManager, UserManager<SUser> userManager)
		{
			string roleName = "Admin";
			string userName = "admin@admin.com";
			string password = "Admin1!@";

			if (!await roleManager.RoleExistsAsync(roleName))
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}

			var user = await userManager.FindByNameAsync(userName);
			if (user == null)
			{
				user = new SUser { FullName = roleName, UserName = userName, Email = userName,EmailConfirmed=true };
				await userManager.CreateAsync(user, password);
			}

			if (!await userManager.IsInRoleAsync(user, roleName))
			{
				await userManager.AddToRoleAsync(user, roleName);
			}
		}
	}
}