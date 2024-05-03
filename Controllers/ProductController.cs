using Computer_Store.Models;
using Computer_Store.Rpo_models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Computer_Store.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ProductController : Controller
	{
		private readonly IRepository<Product> prod;
		private readonly IRepository<Category> categ;
		private readonly IRepository<Photo> photo;

		public ProductController(IRepository<Product> prod, IRepository<Category> categ, IRepository<Photo> photo)
		{
			this.prod = prod;
			this.categ = categ;
			this.photo = photo;
		}
		public async Task<IActionResult> Index()
		{
			return View(await prod.GetAllWithIncludes(d => d.Category,dd=>dd.Discounts));
		}
		public async Task<IActionResult> Create()
		{
			ViewBag.cat = new SelectList(await categ.GetAll(), "CategoryID", "Category_Name");
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Product product, List<IFormFile> photos)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.cat = new SelectList(await categ.GetAll(), "CategoryID", "Category_Name");
				return View(product);
			}
			if (photos.Count==0)
			{
				ViewBag.cat = new SelectList(await categ.GetAll(), "CategoryID", "Category_Name",product.CategoryID);

				ModelState.AddModelError(string.Empty, "ادخل صور المنتج");
				return View(product);
			}
			foreach (var photo in photos)
			{
				if (photo.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await photo.CopyToAsync(memoryStream);
						product.Photos.Add(new Photo { Url = memoryStream.ToArray() });
					}
				}
			}

			product.Add_Time= DateOnly.FromDateTime(DateTime.Now);
				await prod.Create(product);

				return RedirectToAction(nameof(Create));
			}


		public async Task<IActionResult> Edit(int id)
		{
			var pd = await prod.GetById(id);
			if (pd == null)
			{
				return NotFound();
			}
			ViewBag.cat = new SelectList(await categ.GetAll(), "CategoryID", "Category_Name",pd.ProductId);
			return View(pd);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Product product, List<IFormFile> photos)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (photos.Count!=0)
			{
				await photo.DeleteWhere(ss=>ss.ProductId==product.ProductId);
			foreach (var photo in photos)
			{
				if (photo.Length > 0)
				{
					using (var memoryStream = new MemoryStream())
					{
						await photo.CopyToAsync(memoryStream);
						product.Photos.Add(new Photo { Url = memoryStream.ToArray() });
					}
				}
			}
			}

			await prod.Update(product);
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Delete(int id)
		{
			var pd = await prod.GetById(id);
			if (pd == null)
			{
				return NotFound();
			}
			await prod.Delete(pd);
			return RedirectToAction(nameof(Index));
		}

	}

	}

