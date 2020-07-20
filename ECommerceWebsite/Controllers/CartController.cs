using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceWebsite.Data;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebsite.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _ctx;

        public CartController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public ActionResult Index()
        {
            ViewBag.ProductList = HttpContext.Session.GetComplexData<List<Item>>("cart");
            return View();
        }

        public ActionResult Buy(IFormCollection form)
        {

            Product productModel = new Product();
            if (HttpContext.Session.GetComplexData<List<Item>>("cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Product = _ctx.Products.Find(Int32.Parse(form["id"])), Quantity = Int32.Parse(form["Quantity"]) });

                HttpContext.Session.SetComplexData("cart", cart);

            }
            else
            {
                List<Item> cart = HttpContext.Session.GetComplexData<List<Item>>("cart");
                int index = isExist(form["id"]);

                if (index == -1)
                {
                    cart.Add(new Item { Product = _ctx.Products.Find(Int32.Parse(form["id"])), Quantity = Int32.Parse(form["Quantity"]) });
                    HttpContext.Session.SetComplexData("cart", cart);
                }
                else
                {
                    var name = _ctx.Products.Find(Int32.Parse(form["id"]));
                    TempData["abc"] = string.Format("ItemAlreadyExistInTheCart('{0}');", name.Name);
                }

            }

            return RedirectToAction("Index");

        }

        public ActionResult Remove(string id)
        {
            List<Item> cart = HttpContext.Session.GetComplexData<List<Item>>("cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            HttpContext.Session.SetComplexData("cart", cart);
            return RedirectToAction("Index");
        }

        private int isExist(string id)
        {
            List<Item> cart = HttpContext.Session.GetComplexData<List<Item>>("cart");
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].Product.Id == Int32.Parse(id))
                    return i;
            return -1;
        }
    }
}