// DiscountService.cs

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Computer_Store.Data;
using Computer_Store.Models;
using Microsoft.EntityFrameworkCore;
public class DiscountService : BackgroundService
{
	private readonly IServiceProvider _services;
	private readonly ILogger<DiscountService> _logger;

	public DiscountService(IServiceProvider services, ILogger<DiscountService> logger)
	{
		_services = services;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using (var scope = _services.CreateScope())
				{
					var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

					// Get discounts that have ended
					var expiredDiscounts = dbContext.Discount
									.Where(d => d.Discount_End_Time <= DateTime.Now && d.DiscountState==State.Working).Include(ss=>ss.Product)
									.ToList();


					foreach (var item in expiredDiscounts)
					{
						item.Product.HasDiscount = false;
					}
					dbContext.Discount.RemoveRange(expiredDiscounts);
					await dbContext.SaveChangesAsync();
				}
				await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while processing expired discounts.");
			}
		}
	}
}
