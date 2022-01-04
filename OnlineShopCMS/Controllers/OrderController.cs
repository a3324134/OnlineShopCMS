using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopCMS.Data;
using OnlineShopCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopCMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class OrderController : Controller
    {
        private readonly OnlineShopCMSContext _context;
        public OrderController(OnlineShopCMSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? isPaid)
        {
            List<OrderViewModel> orderVM = new List<OrderViewModel>();

            List<Order> orders = null;
            if (isPaid == null)
            {
                orders = await _context.Order.
                    OrderByDescending(k => k.OrderDate).ToListAsync();
            }
            else
            {
                bool paid = false;
                if (isPaid == "1")
                    paid = true;
                orders = await _context.Order.Where(o => o.isPaid == paid)
                    .OrderByDescending(k => k.OrderDate).ToListAsync();
            }

            foreach (var item in orders)
            {
                item.OrderItem = await _context.OrderItem.
                                    Where(p => p.OrderId == item.Id).ToListAsync();
                var vm = new OrderViewModel()
                {
                    Order = item,
                    OrderItemDetails = GetOrderItems(item.Id)
                };
                orderVM.Add(vm);
            }
            return View(orderVM);
        }

        // 取得商品詳細資料
        private List<OrderItemDetail> GetOrderItems(int orderId)
        {
            var OrderItems = _context.OrderItem.Where(p => p.OrderId == orderId).ToList();
            List<OrderItemDetail> orderItems = new List<OrderItemDetail>();
            foreach (var orderitem in OrderItems)
            {
                OrderItemDetail item = new OrderItemDetail(orderitem);
                item.Product = _context.Product.Single(x => x.Id == orderitem.ProductId);
                orderItems.Add(item);
            }
            return orderItems;
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == Id);
            order.OrderItem = await _context.OrderItem.
                                    Where(p => p.OrderId == Id).ToListAsync();
            ViewBag.orderItems = GetOrderItems(order.Id);

            return View(order);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateOrder(Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        order.OrderDate = DateTime.Now;    //取得當前時間
        //        order.isPaid = false;              //付款狀態預設為False
        //        order.OrderItem = SessionHelper.   //綁定訂單內容
        //            GetObjectFromJson<List<OrderItem>>(HttpContext.Session, "cart");

        //        _context.Add(order);               //將訂單寫入資料庫
        //        await _context.SaveChangesAsync();
        //        SessionHelper.Remove(HttpContext.Session, "cart");

        //        //完成後畫面移轉至ReviewOrder()
        //        return RedirectToAction("ReviewOrder", new { Id = order.Id });
        //    }
        //    return View("Checkout");
        //}


    }
}
