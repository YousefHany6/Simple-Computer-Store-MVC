using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Computer_Store.Data;
using Computer_Store.Models;
using Computer_Store.Rpo_models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.IO.Pipelines;
using NuGet.Protocol;

namespace Computer_Store.Controllers
{
 [Authorize(Roles = "Admin")]

 public class CategorieController : Controller
 {
  private readonly IRepository<Category> categ;

  public CategorieController(IRepository<Category> categ)
  {
   this.categ = categ;
  }

  // GET: Categorie
  public async Task<IActionResult> Index()
  {
   return View(await categ.GetAll());
  }


  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(Category category, IFormFile photos)
  {
   if (ModelState.IsValid)
   {
    if (category == null)
    {
     return BadRequest(ModelState);
    }
    if (photos.Length > 0)
    {
     using (var memoryStream = new MemoryStream())
     {
      await photos.CopyToAsync(memoryStream);
      category.photocat = memoryStream.ToArray();
     }
    }
    else
    {
     ModelState.AddModelError(string.Empty,"ادخل صورة للقسم");
     return View(nameof(Index), await categ.GetAll());
    }
    await categ.Create(category);

    return RedirectToAction(nameof(Index));
   }
   ModelState.AddModelError(string.Empty, "ادخل صورة للقسم");
   return View(nameof(Index), await categ.GetAll());
  }

  public async Task<IActionResult> Edit(int id)
  {
   if (id == null)
   {
    return NotFound();
   }

   var category = await categ.GetById(id);
   if (category == null)
   {
    return NotFound();
   }
   return View(category);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id,[Bind("CategoryID,Category_Name")] Category category,IFormFile? photos)
  {
   if (id != category.CategoryID)
   {
    return NotFound();
   }

   if (ModelState.IsValid)
   {
    try
    {
     if (photos!=null)
     {
      using (var memoryStream = new MemoryStream())
      {
       await photos.CopyToAsync(memoryStream);
       category.photocat = memoryStream.ToArray();
      }
     }
     else
     {
      var cate = await categ.GetById(id);
      category.photocat = cate.photocat;
      categ.Detach(cate);

     }

     await categ.Update(category);
    }
    catch (DbUpdateConcurrencyException)
    {
     var check = await categ.GetFilteredAsync(filter => filter.CategoryID == category.CategoryID);
     if (check == null)
     {
      return NotFound();
     }
     else
     {
      throw;
     }
    }
    return RedirectToAction(nameof(Index));
   }
   return View(category);
  }

  public async Task<IActionResult> DeleteConfirmed(int id)
  {
   var category = await categ.GetById(id);
   if (category != null)
   {
    await categ.Delete(category);
   }

   return RedirectToAction(nameof(Index));
  }

 }
}
