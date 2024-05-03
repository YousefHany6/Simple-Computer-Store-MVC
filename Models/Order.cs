using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Models
{
 public class Order
 {

  public int OrderID { get; set; }
  public DateTime OrderDate { get; set; }
  public int CustomerID { get; set; }
  public OrderState State { get; set; } = OrderState.Pending;
  [DataType(DataType.Text)]
  [Required(ErrorMessage ="ادخل عنوانك")]
  public string address { get; set; }
  [DataType(DataType.PhoneNumber)]
  [Required(ErrorMessage = "ادخل رقم هاتفك")]

  public string PhoneNumber { get; set; }
  public decimal TotaPrice { get; set; }
  public Customer? Customer { get; set; }
  public virtual ICollection<OrderList>? OrderLists { get; set; }=new List<OrderList>();
 }
 public enum OrderState
 {
  Pending,
  Shipped,
  Delivered,
  refuse,
		OK,
		OK_Delivered,
  cancel

	}
}
