using ShoppinMVC6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoppinMVC6.Controllers
{
    public class HomeController : Controller
    {
        ShoppingCartEntities db = new ShoppingCartEntities();

        public ActionResult Index()
        {
            Session["userID"] = 1;
            //keeps the record of the items
            List<Carts> lis2 = TempData["cart"] as List<Carts>;
            float total = 0;
            if (TempData["cart"] != null)
            {
                foreach (var item in lis2)
                {
                    total += item.Bill;
                }

                TempData["total"] = total;
            }
            TempData.Keep();

            return View(db.tbl_product.OrderByDescending(a => a.pro_ID));
        }

        public ActionResult AddToCart(int? id)
        {

            var DetailsElement = db.tbl_product.Where(q => q.pro_ID == id).SingleOrDefault();
            return View(DetailsElement);
        }
        List<Carts> lis1 = new List<Carts>();
        [HttpPost]
        public ActionResult AddToCart(int id, int qty)
        {
            TempData.Remove("Mess");
            var p = db.tbl_product.Where(a => a.pro_ID == id).SingleOrDefault();
            Carts cart = new Carts();
            cart.CarttID = p.pro_ID;
            cart.CartName = p.pro_desc;
            cart.price = (float)p.pro_price;
            cart.Quantity = qty;
            cart.Bill = qty * cart.price;

            if (TempData["cart"] == null)
            {
                lis1.Add(cart);
                TempData["cart"] = lis1;
                TempData.Keep();
            }
            else
            {
                List<Carts> lis2 = TempData["cart"] as List<Carts>;
                var flag = 0;
                foreach (var item in lis2)
                {
                    if (item.CarttID == cart.CarttID)
                    {
                        item.Quantity += cart.Quantity;
                        item.Bill += cart.Bill;
                        flag = 1;
                    }
                }
                if (flag == 0)
                {
                    lis2.Add(cart);
                }
                TempData["cart"] = lis2;
                TempData.Keep();
            }

            return RedirectToAction("Index");
        }
        public ActionResult ChekToCart()
        {

            TempData.Keep();
            return View();
        }
        [HttpPost]
        public ActionResult ChekToCart(tbl_order Order)
        {
            List<Carts> lis2 = TempData["cart"] as List<Carts>;

            tbl_invoice inv = new tbl_invoice();
            inv.inv_date = DateTime.Now;
            inv.inv_fk_use = (int)Session["userID"];
            inv.inv_total = Convert.ToDouble(TempData["total"]);
            db.tbl_invoice.Add(inv);
            db.SaveChanges();

            foreach (var item in lis2)
            {

                tbl_order O = new tbl_order();
                O.o_date = DateTime.Now;
                O.o_fk_inv = inv.inv_ID;
                O.o_fk_pro = item.CarttID;
                O.o_quty = item.Quantity;
                O.o_unitprice = (int)item.price;
                O.o_bill = item.Bill;

                db.tbl_order.Add(O);
                db.SaveChanges();

            }

            TempData.Remove("cart");
            TempData.Remove("total");
            TempData["Mess"] = "successful transection ...";
            TempData.Keep();
            return View();
        }
        public ActionResult Remove(int? id)
        {
            List<Carts> lis2 = TempData["cart"] as List<Carts>;
            TempData["cart"] = lis2;
            if (TempData["cart"] == null)
            {
                TempData.Remove("cart");
                return View("ChekToCart");
            }
            else
            {
                var RE = lis2.Where(a => a.CarttID == id).SingleOrDefault();


                lis2.Remove(RE);
                float b = 0;
                foreach (var item in lis2)
                {
                    b += item.Bill;
                }
                TempData["total"] = b;

                return View("ChekToCart");
            }

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}