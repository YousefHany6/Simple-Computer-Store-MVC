// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Computer_Store.Data;
using Computer_Store.Models;
using Computer_Store.Rpo_models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Computer_Store.Areas.Identity.Pages.Account.Manage
{
 public class IndexModel : PageModel
 {
  private readonly UserManager<SUser> _userManager;
  private readonly SignInManager<SUser> _signInManager;
  private readonly IRepository<Customer> repositorycust;

  public IndexModel(
      UserManager<SUser> userManager,
      SignInManager<SUser> signInManager,
      IRepository<Customer> repositorycust
   )
  {
   _userManager = userManager;
   _signInManager = signInManager;
   this.repositorycust = repositorycust;
  }

  /// <summary>
  ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
  ///     directly from your code. This API may change or be removed in future releases.
  /// </summary>
  public string Username { get; set; }

  /// <summary>
  ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
  ///     directly from your code. This API may change or be removed in future releases.
  /// </summary>
  [TempData]
  public string StatusMessage { get; set; }

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
  public class InputModel
  {
   /// <summary>
   ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
   ///     directly from your code. This API may change or be removed in future releases.
   /// </summary>
   [Required(ErrorMessage ="ادخل اسمك الجديد")]
   public string FullName { get; set; }
  }

  private async Task LoadAsync(SUser user)
  {
   var userName = await _userManager.GetUserNameAsync(user);
   var us = await _userManager.GetUserAsync(User);

   Username = userName;

   Input = new InputModel
   {
    FullName = us.FullName
   };
  }

  public async Task<IActionResult> OnGetAsync()
  {
   var user = await _userManager.GetUserAsync(User);
   if (user == null)
   {
    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
   }

   await LoadAsync(user);
   return Page();
  }

  public async Task<IActionResult> OnPostAsync()
  {
   var user = await _userManager.GetUserAsync(User);
   if (user == null)
   {
    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
   }

   if (!ModelState.IsValid)
   {
    await LoadAsync(user);
    return Page();
   }

   var uus = await _userManager.GetUserAsync(User);
   if (Input.FullName != uus.FullName)
   {
    var c = await repositorycust.filterone(s => s.userid == uus.Id);
    c.FullName=Input.FullName;
    await repositorycust.Update(c);

				uus.FullName=Input.FullName.TrimStart().TrimEnd();
    var setPhoneResult = await _userManager.UpdateAsync(user);

    if (!setPhoneResult.Succeeded)
    {
     StatusMessage = "خطأ";
     return RedirectToPage();
    }
   }

   await _signInManager.RefreshSignInAsync(user);
   StatusMessage = "تم تحديث ملفك الشخصي";
   return RedirectToPage();
  }
 }
}
