using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Models
{
	public class CartItem
	{
		[Key]

		public int CartItemId { get; set; }

		public int ProductId { get; set; }
		public virtual Product? Product { get; set; }
		public int Quentity { get; set; } = 1;
		public int CartId { get; set; }
		public virtual Cart? Cart { get; set; }
	}
}
