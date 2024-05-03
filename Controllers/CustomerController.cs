using Computer_Store.Data;
using Computer_Store.Models;
using Computer_Store.Rpo_models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Computer_Store.Controllers
{
	public class CustomerController : Controller
	{
		private readonly SignInManager<SUser> signIn;
		private readonly UserManager<SUser> userManager;
		private readonly IRepository<Product> prod;
		private readonly IRepository<Customer> custt;
		private readonly IRepository<OrderList> orderlist;
		private readonly IRepository<Order> order;
		private readonly IRepository<CartItem> cri;
		private readonly IRepository<Cart> cr;
		private readonly IRepository<Customer> ctt;
		private readonly IRepository<Category> categ;
		private readonly IRepository<Photo> photo;

		public CustomerController(SignInManager<SUser> signIn, UserManager<SUser> userManager, IRepository<Product> prod, IRepository<Customer> custt, IRepository<OrderList> orderlist, IRepository<Order> Order, IRepository<CartItem> cri, IRepository<Cart> cr, IRepository<Customer> ctt, IRepository<Category> categ, IRepository<Photo> photo)
		{
			this.signIn = signIn;
			this.userManager = userManager;
			this.prod = prod;
			this.custt = custt;
			this.orderlist = orderlist;
			order = Order;
			this.cri = cri;
			this.cr = cr;
			this.ctt = ctt;
			this.categ = categ;
			this.photo = photo;
		}
		public async Task<IActionResult> Index()
		{
			if (User.IsInRole("Admin"))
			{
				return RedirectToAction("Index", "Categorie");
			}
			return View(await prod.GetAllWithIncludes(
				s=>s.Photos,
				ss=>ss.Discounts
				));
		}
		public async Task<IActionResult> cart()
		{
			if (!(User.Identity.IsAuthenticated && signIn.IsSignedIn(User)))
			{
				return LocalRedirect("/Identity/Account/Login");
			}

			var us = await userManager.GetUserAsync(User);
			var cust = await ctt.filterone(ss => ss.userid == us.Id);
			if (cust == null)
			{
				return NotFound();
			}
			var customer = await custt.filterone(ss => ss.userid == us.Id);
			ViewBag.ccust = customer.CustomerId;
			var c = await cr.filterone(s => s.CustomerId == cust.CustomerId);
			return View(await cri.GetFilteredAsync(
				s => s.CartId == c.CartId,
				sss => sss.Product.Photos
			, sd => sd.Product.Category,
				ssd => ssd.Product.Discounts,
				ssd => ssd.Cart
			)
				);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CartList(int idprd, int cart_quantity, int catid)
		{
			if (!User.Identity.IsAuthenticated || !signIn.IsSignedIn(User))
			{
				return LocalRedirect("/Identity/Account/Login");
			}

			var us = await userManager.GetUserAsync(User);
			var cust = await ctt.filterone(ss => ss.userid == us.Id);

			if (cust == null)
			{
				return NotFound();
			}

			var c = await cr.filterone(s => s.CustomerId == cust.CustomerId);
			var checkitem = await cri.filterone(s => s.CartId == c.CartId && s.ProductId == idprd);

			if (checkitem != null)
			{
				if (cart_quantity != null && cart_quantity > 0)
				{
					checkitem.Quentity = cart_quantity;
					await cri.Update(checkitem);
					return RedirectToAction(nameof(product_page), new { prdid = idprd });

				}
				if (catid==0)
				{
     return RedirectToAction(nameof(product_page), new { prdid = idprd });

    }
    return RedirectToAction(nameof(catgory_page), new { catid = catid });
			}

			if (cart_quantity == null || cart_quantity == 0)
			{
				await cri.Create(new CartItem { ProductId = idprd, CartId = c.CartId, Quentity = 1 });
				if (catid==0)
				{
     return RedirectToAction(nameof(product_page), new { prdid = idprd });

    }

   }

   else
   {
				await cri.Create(new CartItem { ProductId = idprd, CartId = c.CartId, Quentity = cart_quantity });
				return RedirectToAction(nameof(product_page), new { prdid = idprd });
			}

   return RedirectToAction(nameof(catgory_page), new { catid = catid });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Carthome(int idprd, int cart_quantity)
		{
			if (!User.Identity.IsAuthenticated || !signIn.IsSignedIn(User))
			{
				return LocalRedirect("/Identity/Account/Login");
			}

			var us = await userManager.GetUserAsync(User);
			var cust = await ctt.filterone(ss => ss.userid == us.Id);

			if (cust == null)
			{
				return NotFound();
			}

			var c = await cr.filterone(s => s.CustomerId == cust.CustomerId);
			var checkitem = await cri.filterone(s => s.CartId == c.CartId && s.ProductId == idprd);

			if (checkitem != null)
			{
				if (cart_quantity != null && cart_quantity > 0)
				{
					checkitem.Quentity = cart_quantity;
					await cri.Update(checkitem);
				}

				return RedirectToAction(nameof(Index));
			}

			if (cart_quantity == null || cart_quantity == 0)
			{
				await cri.Create(new CartItem { ProductId = idprd, CartId = c.CartId, Quentity = 1 });
			}
			else
			{
				await cri.Create(new CartItem { ProductId = idprd, CartId = c.CartId, Quentity = cart_quantity });
			}

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> delete_item(int pdid, int cartid)
		{

			await cri.DeleteWhere(cri => cri.CartId == cartid && cri.ProductId == pdid);
			return RedirectToAction(nameof(cart));
		}



		public async Task<IActionResult> product_page(int prdid)
		{
			if (prdid == 0)
			{
				return BadRequest(ModelState);
			}
			ViewBag.sproducts = await prod.GetAllWithIncludes(ss => ss.Discounts, sd => sd.Photos);
			if (!(User.Identity.IsAuthenticated || signIn.IsSignedIn(User)))
			{
				return View(await prod.GetProductAsync(prdid));
			}

			var us = await userManager.GetUserAsync(User);
			var customer = await custt.filterone(ss => ss.userid == us.Id);
			ViewBag.ccust = customer.CustomerId;


			return View(await prod.GetProductAsync(prdid));
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Confirm_Order(string address, string phoneNumber, int Cid, decimal totalPrice, List<int> productid, List<int> qs)
		{
			if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(phoneNumber))
			{
				var or = new Order
				{
					CustomerID = Cid,
					OrderDate = DateTime.Now,
					TotaPrice = totalPrice,
					address = address,
					PhoneNumber = phoneNumber
				};

				await order.Create(or);

				foreach (var (item, qq) in productid.Zip(qs, (item, qq) => (item, qq)))
				{
					var orlist = new OrderList
					{
						OrderID = or.OrderID,
						ProductID = item,
						Quantity = qq
					};

					await orderlist.Create(orlist);
				}
			}

			return RedirectToAction(nameof(cart));
		}

		public async Task<IActionResult> My_Orders()
		{
			var us = await userManager.GetUserAsync(User);
			if (us == null)
			{
				return NotFound();
			}
			var customer = await custt.filterone(ss => ss.userid == us.Id);

			return View(await order.GetFilteredAsync(ss => ss.CustomerID == customer.CustomerId, ss => ss.Customer
			)
				);

		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Cancel_Order(int orderId)
		{
			if (orderId == 0)
			{
				return BadRequest();
			}

			var or = await order.GetById(orderId);

			if (or == null)
			{
				return NotFound();
			}

			or.State = OrderState.cancel;
			await order.Update(or);

			return RedirectToAction(nameof(My_Orders));
		}

		public async Task<IActionResult> order_Details(int orderid)
		{
			if (orderid == 0)
			{
				return BadRequest();
			}
			return View(await orderlist.GetFilteredAsync(s=>s.OrderID==orderid,sd=>sd.Product.Photos,a=>a.Product.Discounts,m=>m.Order,d=>d.Product.Category));
		}


		public async Task<IActionResult> catgory_page(int catid)
		{
			return View(await prod.GetFilteredAsync(ss => ss.CategoryID == catid, s => s.Category, sx => sx.Photos, sf => sf.Discounts));
		}

		public async Task<IActionResult> Contact()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Sereach(string name)
		{
			return View("catgory_page", await prod.Sereach(name));
		}
	}	
}
