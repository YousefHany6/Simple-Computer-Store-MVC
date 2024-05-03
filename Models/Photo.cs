using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Models
{
	public class Photo
	{
		[Key]
		public int PhotoId { get; set; }
		public byte[] Url { get; set; }

		public int ProductId { get; set; }
		public virtual Product? Product { get; set; }
	}

}

