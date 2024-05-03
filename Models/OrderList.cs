namespace Computer_Store.Models
{
 public class OrderList
 {
  public int OrderID { get; set; }
  public int ProductID { get; set; }
  public int Quantity { get; set; }

  public Order? Order { get; set; }
  public Product? Product { get; set; }
 }
}
