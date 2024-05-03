// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Computer_Store.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Computer_Store.Models;
using Computer_Store.Rpo_models;

namespace Computer_Store.Areas.Identity.Pages.Account
{
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<SUser> _signInManager;
		private readonly UserManager<SUser> _userManager;
		private readonly IUserStore<SUser> _userStore;
		private readonly IUserEmailStore<SUser> _emailStore;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;
		private readonly IRepository<Customer> customer;
		private readonly IRepository<Cart> ct;

		public RegisterModel(
						UserManager<SUser> userManager,
						IUserStore<SUser> userStore,
						SignInManager<SUser> signInManager,
						ILogger<RegisterModel> logger,
						IEmailSender emailSender,
						IRepository<Customer> customer,
						IRepository<Cart> ct
						)
		{
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
			this.customer = customer;
			this.ct = ct;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string ReturnUrl { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			/// 	
			[Required(ErrorMessage="ادخل الاسم كامل")]
			[MaxLength(100)]
			public string FullName { get; set; }
			[Required(ErrorMessage="ادخل البريد الإلكتروني")]
			[EmailAddress(ErrorMessage = " ادخل بريد الإلكتروني صحيح")]
			public string Email { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Required(ErrorMessage="ادخل كلمة المرور")]
			[DataType(DataType.Password),MinLength(8,ErrorMessage ="الحد الادنى 8 حروف")]
			public string Password { get; set; }

			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[DataType(DataType.Password), MinLength(8, ErrorMessage = "الحد الادنى 8 حروف")]
			[Required(ErrorMessage = "ادخل كلمة المرور")]

			[Compare("Password", ErrorMessage = "كلمة السر غير مطابقة")]
			public string ConfirmPassword { get; set; }
		}


		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				var user = CreateUser();

				await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
				user.FullName = Input.FullName;
				var result = await _userManager.CreateAsync(user, Input.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					var cust = new Customer
					{
						FullName = Input.FullName,
						Email = Input.Email,
						userid = user.Id
					};
					await customer.Create(cust);

					var cart = new Cart
					{
						CustomerId=cust.CustomerId
					};
					await ct.Create(cart);

					var userId = await _userManager.GetUserIdAsync(user);
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
									"/Account/ConfirmEmail",
									pageHandler: null,
									values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
									protocol: Request.Scheme);

					await _emailSender.SendEmailAsync(Input.Email, "قم بتأكيد بريدك الإلكتروني",
									$"يرجى تأكيد حسابك عن طريق <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>اضغط هنا</a>.");

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(returnUrl);
					}
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
			return Page();
		}

		private SUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<SUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(SUser)}'. " +
								$"Ensure that '{nameof(SUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
								$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		private IUserEmailStore<SUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<SUser>)_userStore;
		}
	}
}
