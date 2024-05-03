using Computer_Store.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection.Emit;

namespace Computer_Store.Data
{
	public class ApplicationDbContext : IdentityDbContext<SUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
																		: base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);


			builder.Entity<OrderList>()
							.HasKey(od => new { od.OrderID, od.ProductID });

			builder.Entity<OrderList>()
							.HasOne(od => od.Order)
							.WithMany(o => o.OrderLists)
							.HasForeignKey(od => od.OrderID).OnDelete(DeleteBehavior.Cascade);


			builder.Entity<OrderList>()
							.HasOne(od => od.Product)
							.WithMany(p => p.OrderLists)
							.HasForeignKey(od => od.ProductID)
														.OnDelete(DeleteBehavior.Cascade);


			builder.Entity<Order>()
							.HasOne(o => o.Customer)
							.WithMany(c => c.Orders)
							.HasForeignKey(o => o.CustomerID).OnDelete(DeleteBehavior.Cascade);


			builder.Entity<Order>()
							.Property(o => o.State)
							.HasConversion<string>();
			builder.Entity<Order>()
									.Property(p => p.TotaPrice)
									.HasColumnType("decimal(18,2)");

			builder.Entity<Product>()
								.Property(p => p.price)
								.HasColumnType("decimal(18,2)");
			builder.Entity<Discount>()
								.Property(p => p.DiscountAmount)
								.HasColumnType("decimal(18,2)");

			builder.Entity<Discount>()
							.HasOne(p => p.Product)
							.WithMany(c => c.Discounts)
							.HasForeignKey(p => p.ProductId)
							.OnDelete(DeleteBehavior.Cascade);


			builder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryID)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Photo>()
											.HasOne(p => p.Product)
											.WithMany(pr => pr.Photos)
											.HasForeignKey(p => p.ProductId)
											.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Cart>()
											.HasOne(c => c.Customer)
											.WithMany(cust => cust.Carts)
											.HasForeignKey(c => c.CustomerId)
																						.OnDelete(DeleteBehavior.Cascade);


			builder.Entity<CartItem>()
							.HasOne(ci => ci.Product)
							.WithMany(p => p.CartItems)
							.HasForeignKey(ci => ci.ProductId)
																		.OnDelete(DeleteBehavior.Cascade);


			builder.Entity<CartItem>()
							.HasOne(ci => ci.Cart)
							.WithMany(c => c.CartItems)
							.HasForeignKey(ci => ci.CartId)
																		.OnDelete(DeleteBehavior.Cascade);


		}
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<Photo> Photos { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Order> orders { get; set; }
		public DbSet<OrderList> orderLists { get; set; }
		public DbSet<Discount> Discount { get; set; } = default!;





	}
}
