using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Models
{
	public class Discount
	{
		[Key]
		public int DiscountId { get; set; }
		[Required(ErrorMessage ="ادخل قيمة الخصم")]
		public decimal DiscountAmount { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime Discount_Time_start { get; set; }
		[Required(ErrorMessage ="ادخل تاريخ الانتهاء")]
		[DataType(DataType.DateTime)]
		public DateTime? Discount_End_Time { get; set; }

		[Required]
		public State DiscountState { get; set; }

		public int ProductId { get; set; }
		public virtual Product? Product { get; set; }
	}
	 public enum State{
		expired,
		Working
	}

}
