using Computer_Store.Data;
using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Models
{
	public class Customer
	{
		[Key]
		public int CustomerId { get; set; }
		[Required(ErrorMessage ="ادخل الاسم الاول")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "ادخل الايميل")]
		public string Email { get; set; }
		public string userid { get; set; }
		public virtual ICollection<Cart>? Carts { get; set; } = new HashSet<Cart>();
  public virtual ICollection<Order>? Orders { get; set; }= new HashSet<Order>();


 }
}
