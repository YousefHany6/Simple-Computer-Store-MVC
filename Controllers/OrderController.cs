using Computer_Store.Models;
using Computer_Store.Rpo_models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Computer_Store.Controllers
{
	[Authorize(Roles = "Admin")]

	public class OrderController : Controller
	{
		private readonly IRepository<Order> ord;
		private readonly IRepository<OrderList> ordlist;
		private readonly IRepository<Product> prd;

		public OrderController(IRepository<Order> ord, IRepository<OrderList> ordlist, IRepository<Product> prd)
		{
			this.ord = ord;
			this.ordlist = ordlist;
			this.prd = prd;
		}
		public async Task<IActionResult> Index()
		{
			return View(await ord.GetAllWithIncludes(
				ss=>ss.Customer,
				sd=>sd.OrderLists
				
				));
		}
		public async Task<IActionResult> PendingOrder()
		{
			return View(await ord.GetAllWithIncludes(
				ss => ss.Customer,
				sd => sd.OrderLists

				));
		}
  [HttpPost]
		[ValidateAntiForgeryToken]
  public async Task<IActionResult> UpdateOrderStatus(OrderState ordstate, int[] selectedOrderIds)
  {
			if (selectedOrderIds.Length==0)
			{
				ModelState.AddModelError(string.Empty, "يرجى اختيار طلب");
				return View("PendingOrder", await ord.GetFilteredAsync(aas => aas.State == OrderState.Pending,
    ss => ss.Customer,
    sd => sd.OrderLists

    ));
			}
			
			foreach (int id in selectedOrderIds)
			{
				var orderid = await ord.GetById(id);
				orderid.State = ordstate;
				await ord.Update(orderid);
				if (ordstate==OrderState.OK_Delivered)
				{

				var orderlistt = await ordlist.GetFilteredAsync(sd => sd.OrderID == id);
				foreach (var item in orderlistt)
				{
					var p = await prd.GetById(item.ProductID);
					p.quantity= p.quantity - item.Quantity;
					await prd.Update(p);

				}
				}



			}

			return RedirectToAction(nameof(PendingOrder));
  }

 }
}
