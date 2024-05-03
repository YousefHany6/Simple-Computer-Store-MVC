using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Models
{
	public class Product
	{
		[Key]
		public int ProductId { get; set; }
		[Required(ErrorMessage = "ادخل اسم المنتج")]
		public string Product_Name { get; set; }
		[Required(ErrorMessage = "ادخل سعر المنتج")]
		public decimal price { get; set; }
		[DataType(DataType.DateTime)]
		public DateOnly? Add_Time { get; set; } 
		[Required(ErrorMessage = "اختر القسم")]
		public int CategoryID { get; set; }
		public virtual Category? Category { get; set; }
		public virtual ICollection<Discount>? Discounts { get; set; } = new HashSet<Discount>();

		[Required(ErrorMessage = "ادخل الكمية المتوفرة للمنتج")]

		public int quantity { get; set; }
		public string? Description { get; set; }
		public virtual ICollection<CartItem>? CartItems { get; set; } = new HashSet<CartItem>();
		public bool HasDiscount { get; set; }=false;

		public virtual ICollection<Photo>? Photos { get; set; } = new HashSet<Photo>();
		[Required(ErrorMessage = "ادخل نوع المنتج")]
		public string Type { get; set; }

  public virtual ICollection<OrderList>? OrderLists { get; set; }=new HashSet<OrderList>();

 }
}
