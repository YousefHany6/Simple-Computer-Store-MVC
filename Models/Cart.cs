using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Models
{
	public class Cart
	{
		[Key]

		public int CartId { get; set; }

		public int CustomerId { get; set; }
		public virtual Customer? Customer { get; set; }

		public virtual ICollection<CartItem>? CartItems { get; set; }=new HashSet<CartItem>();
	}
}
