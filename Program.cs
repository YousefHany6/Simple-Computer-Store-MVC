using Computer_Store.Data;
using Computer_Store.Rpo_models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using Computer_Store.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Computer_Store
{
 public class Program
 {
  public static  void Main(string[] args)
  {
   var builder = WebApplication.CreateBuilder(args);

   var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
   builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseSqlServer(connectionString));

   builder.Services.AddDatabaseDeveloperPageExceptionFilter();

   builder.Services.Configure<RequestLocalizationOptions>(options =>
   {
    options.DefaultRequestCulture = new RequestCulture("ar");
    options.SupportedCultures = new List<CultureInfo> { new CultureInfo("ar") };
    options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("ar") };
   });

   builder.Services.AddIdentity<SUser, IdentityRole>(options=>options.SignIn.RequireConfirmedAccount=true)
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultUI()
       .AddDefaultTokenProviders();

   builder.Services.AddControllersWithViews();
   builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

   builder.Services.AddHostedService<DiscountService>();
   builder.Services.AddTransient<IEmailSender,EmailSender>();

   builder.Services.Configure<IdentityOptions>(options =>
   {
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
   });

   var app = builder.Build();
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
   app.UseStaticFiles();

   app.UseRouting();

   app.UseAuthentication();
   app.UseAuthorization();
   app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Customer}/{action=Index}/{id?}")
;
   app.MapRazorPages();
			Seed_Role_Admin.InitializeAsync(app.Services).Wait();


			app.Run();
  }
 }
}

