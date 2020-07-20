using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ECommerceWebsite.Data;
using ECommerceWebsite.Filters;
using ECommerceWebsite.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebsite.Controllers
{
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public readonly ApplicationDbContext _ctx;

        public ProductController(ApplicationDbContext ctx, IWebHostEnvironment webHostEnvironment)
        {
            _ctx = ctx;
            _webHostEnvironment = webHostEnvironment;
        }


        [AuthorizeRoles("Admin","Retailer")]
        public async Task<IActionResult> ListProduct()
        {
            if (User.IsInRole("Admin") || User.IsInRole("Customer"))
            {
                return View(await _ctx.Products.ToListAsync());
            }
            else
            {
                var RetailerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return View(await _ctx.Products.Where(p => p.RetailerId == RetailerId).ToListAsync());
            }

        }

        [AuthorizeRoles("Customer")]
        public async Task<IActionResult> OrderList()
        {
            ViewData["tmp"] = "tmp";
            return View(await _ctx.Products.ToListAsync());
        }

        [AuthorizeRoles("Customer")]
        public async Task<IActionResult> BuyProduct()
        {
            int totalAmount = 0, totalDiscount = 0;

            foreach (Item item in HttpContext.Session.GetComplexData<List<Item>>("cart"))
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    ProductId = item.Product.Id,
                    Category = item.Product.Category,
                    Price = item.Product.UnitPrice,
                    Quantity = item.Quantity,
                    Unit = item.Product.Unit,
                    TotalAmount = item.Quantity * item.Product.UnitPrice - (((item.Product.Discount * item.Product.UnitPrice) / 100) * item.Quantity),
                    TotalDiscount = ((item.Product.Discount * item.Product.UnitPrice) / 100) * item.Quantity

                };
                totalAmount += orderDetail.TotalAmount;
                totalDiscount += orderDetail.TotalDiscount;

                _ctx.Add(orderDetail);
                await _ctx.SaveChangesAsync();
            }

            int orderNo;
            int customerId = 456;

            orderNo = GetOrderNumber(customerId);


            BookOrder(orderNo, customerId, totalAmount, totalDiscount);

            return RedirectToAction("OrderList");

        }

        public int GetOrderNumber(int customerId)
        {
            int orderNumber = 1;

            if (_ctx.Orders.Find(customerId) != null)
            {
                var cnt = from t in _ctx.Orders
                          where t.CustomerId == customerId
                          select t;
                orderNumber = cnt.Count();

            }

            return orderNumber;
        }


        public void BookOrder(int orderNo, int customerId, int totalAmount, int totalDiscount)
        {
            Order order = new Order
            {
                OrderNumber = orderNo,
                CustomerId = customerId,
                Orderdate = DateTime.Now,
                OrderStatus = "Received",
                TotalAmount = totalAmount,
                TotalDiscount = totalDiscount
            };

            _ctx.Add(order);

            _ctx.SaveChanges();

            List<Item> cart = null;

            HttpContext.Session.SetComplexData("cart", cart);

        }

        [HttpGet]
        [AuthorizeRoles("Admin", "Retailer")]
        public ActionResult CreateProduct()
        {
            ViewBag.userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View();
        }

        [HttpPost]
        [AuthorizeRoles("Admin", "Retailer")]
        public async Task<IActionResult> CreateProduct(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                Product product = new Product
                {
                    RetailerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Name = productVM.Name,
                    Category = productVM.Category,
                    Unit = productVM.Unit,
                    UnitPrice = productVM.UnitPrice,
                    Discount = productVM.Discount,
                    Photo = UploadedFile(productVM.Photo),
                    Status = productVM.Status
                };
                _ctx.Add(product);
                await _ctx.SaveChangesAsync();

                return RedirectToAction(nameof(ListProduct), new { product.RetailerId });

            }

            return View(productVM);
        }


        private string UploadedFile(IFormFile Photo)
        {
            string uniqueFileName = null;

            if (Photo != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }


        [HttpGet]
        [AuthorizeRoles("Admin", "Retailer")]
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _ctx.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        [HttpPost]
        [AuthorizeRoles("Admin", "Retailer")]
        public async Task<IActionResult> EditProduct(int id, IFormFile Photo2, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Photo2 != null)
                    {
                        product.Photo = UploadedFile(Photo2);
                    }
                    _ctx.Update(product);
                    await _ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(_ctx.Products.Any(e => e.Id == id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListProduct), new { product.RetailerId });
            }
            return View(product);
        }


        [HttpGet]
        [AuthorizeRoles("Admin", "Retailer")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _ctx.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [HttpPost, ActionName("DeleteProduct")]
        [AuthorizeRoles("Admin", "Retailer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _ctx.Products.FindAsync(id);

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            string filePath = Path.Combine(uploadsFolder, product.Photo);

            System.IO.File.Delete(filePath);

            _ctx.Products.Remove(product);

            await _ctx.SaveChangesAsync();

            return RedirectToAction(nameof(ListProduct), new { product.RetailerId });
        }
    }
}