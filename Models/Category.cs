using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Models
{
	public class Category
	{
		[Key]
		public int CategoryID { get; set; }

		[Required(ErrorMessage ="ادخل اسم القسم")]
		public string Category_Name { get; set; }
		public byte[]? photocat { get; set; }

		public virtual ICollection<Product>? Products { get; set; }=new HashSet<Product>();
	}
}
