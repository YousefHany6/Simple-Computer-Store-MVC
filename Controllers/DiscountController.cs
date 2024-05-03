using Computer_Store.Models;
using Computer_Store.Rpo_models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Computer_Store.Controllers
{
	[Authorize(Roles = "Admin")]
	public class DiscountController : Controller
	{
		private readonly IRepository<Product> prod;
		private readonly IRepository<Discount> dis;

		public DiscountController(IRepository<Product> prod, IRepository<Discount> dis)
		{
			this.prod = prod;
			this.dis = dis;
		}
  public async Task<IActionResult> Index()
  {
   return View(await dis.GetAllWithIncludes(s=>s.Product));
  }
  public async Task<IActionResult> Create(int id)
		{
			ViewBag.pd = await prod.GetById(id);
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Discount discount)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			discount.Discount_Time_start = DateTime.Now;
			 if (discount.Discount_End_Time.HasValue && discount.Discount_End_Time < DateTime.Now)
    {
        discount.DiscountState = State.expired;
    }
    else
    {
        discount.DiscountState = State.Working;
    }
			var check = await dis.GetFilteredAsync(ss=>ss.ProductId==discount.ProductId&&ss.DiscountState==State.Working);
			if (check.Count!=0)
			{
				ModelState.AddModelError(string.Empty, "يوجد بالفعل خصم لهذا المنتج");
    ViewBag.pd = await prod.GetById(discount.ProductId);
    return View();
   }
			await dis.Create(discount);
   var d=await prod.GetById(discount.ProductId);
			d.HasDiscount = true;
			await prod.Update(d);

   return RedirectToAction("Index", "Product");
		}
		public async Task<IActionResult> Edit(int id,int idpd)
		{
			var dd = await dis.GetById(id);
			if (dd == null)
			{
				return NotFound();
			}
			ViewBag.pd = await prod.GetById(idpd);
			return View(dd);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Discount discount)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (discount.Discount_End_Time.HasValue && discount.Discount_End_Time < DateTime.Now)
			{
				discount.DiscountState = State.expired;
			}
			else
			{
				discount.DiscountState = State.Working;
			}
		
			await dis.Update(discount);
			return RedirectToAction(nameof(Index));
		}

  public async Task<IActionResult> Delete(int id)
  {
   var diss = await dis.GetById(id);
   if (diss == null)
   {
    return NotFound();
   }
			var d = await prod.GetById(diss.ProductId);
			d.HasDiscount = false;
			await prod.Update(d);

			await dis.Delete(diss);
   return RedirectToAction(nameof(Index));
  }


		public async Task<IActionResult> Discount_All_Products()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Discount_All_Products(decimal discount,DateTime date)
		{
			var products = await prod.GetAll();
			if (products == null)
			{
				return NotFound();
			}
			foreach (var item in products)
			{
				if (item.HasDiscount==true)
				{
					continue;
				}
				var originalPrice = item.price;
				var discountAmount = (discount / 100) * originalPrice;
				var discountedPrice = originalPrice - discountAmount;

				var disss = new Discount
				{
					DiscountAmount = discountedPrice,
					DiscountState = State.Working,
					Discount_End_Time = date,
					ProductId = item.ProductId,
					Discount_Time_start = DateTime.Now
					};
				await dis.Create(disss);

				var d = await prod.GetById(item.ProductId);
				d.HasDiscount = true;
				await prod.Update(d);

			}
			return RedirectToAction(nameof(Index));
		}
	}
}
