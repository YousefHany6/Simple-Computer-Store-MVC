using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Computer_Store.Data
{
	public class SUser : IdentityUser
	{
		public string? FullName { get; set; }
		[Required(ErrorMessage = "ادخل ايميل صالح")]
		[DataType(DataType.EmailAddress, ErrorMessage = "ادخل ايميل صالح", ErrorMessageResourceName = "ادخل ايميل صالح")]
		[EmailAddress(ErrorMessage = "ادخل ايميل صالح")]
		public override string Email { get => base.Email; set => base.Email = value; }
		[MinLength(6, ErrorMessage = "الحد الادنى 6 حروف")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "ادخل كلمة المرور صالحة")]
		public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }

	}
}
