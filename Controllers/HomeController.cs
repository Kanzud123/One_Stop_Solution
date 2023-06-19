using WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        dbemarketingEntities1 db = new dbemarketingEntities1();
        public ActionResult Index()
        {
            Session["u_id"] = 1;
            if (TempData["cart"] != null)
            {
                float x = 0;
                List<cart> li2 = TempData["cart"] as List<cart>;
                foreach (var item in li2)
                {
                    x += item.bill;

                }

                TempData["total"] = x;
            }
            TempData.Keep();
            return View(db.tbl_product.OrderByDescending(x => x.pro_id).ToList());
        }

        public ActionResult SignUp()
        {

            return View();
        }



        [HttpPost]
        public ActionResult SignUp(tbl_user uvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_email = uvm.u_email;
                u.u_password = uvm.u_password;
                u.u_image = path;
                u.u_contact = uvm.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");

            }

            return View();
        }








        public ActionResult login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult login(tbl_user avm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (ad != null)
            {

                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.error = "Invalid username or password";

            }

            return View();
        }

        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }














        public ActionResult Adtocart(int? Id)
        {

            tbl_product p = db.tbl_product.Where(x => x.pro_id == Id).SingleOrDefault();
            return View(p);
        }

        List<cart> li = new List<cart>();
        [HttpPost]
        public ActionResult Adtocart(tbl_product pi, string qty, int Id)
        {
            tbl_product p = db.tbl_product.Where(x => x.pro_id == Id).SingleOrDefault();

            cart c = new cart();
            c.productid = p.pro_id;
            c.price = (float)p.pro_price;
            c.qty = Convert.ToInt32(qty);
            c.bill = c.price * c.qty;
            c.productname = p.pro_name;
            if (TempData["cart"] == null)
            {
                li.Add(c);
                TempData["cart"] = li;

            }
            else
            {
                List<cart> li2 = TempData["cart"] as List<cart>;
                int flag = 0;
                foreach (var item in li2)
                {

                    if (item.productid == c.productid)
                    {
                        item.qty += c.qty;
                        item.bill += c.bill;
                        flag = 1;
                    }
                }
                if (flag == 0)
                {

                    li2.Add(c);
                }
                TempData["cart"] = li2;
            }

            TempData.Keep();




            return RedirectToAction("Index");
        }

        public ActionResult remove(int? id)
        {
            List<cart> li2 = TempData["cart"] as List<cart>;
            cart c = li2.Where(x => x.productid == id).SingleOrDefault();
            li2.Remove(c);
            float h = 0;
            foreach (var item in li2)
            {
                h += item.bill;

            }
            TempData["total"] = h;
            return RedirectToAction("checkout");
        }




        public ActionResult checkout()
        {
            TempData.Keep();


            return View();
        }
        [HttpPost]
        public ActionResult checkout(tbl_order O)
        {
            List<cart> li = TempData["cart"] as List<cart>;

            tbl_invoice iv = new tbl_invoice();
            iv.in_fk_user = Convert.ToInt32(Session["u_id"].ToString());
            iv.in_date = System.DateTime.Now;
            iv.in_totalbill = (float)TempData["total"];
            db.tbl_invoice.Add(iv);
            db.SaveChanges();

            foreach (var item in li)
            {
                tbl_order od = new tbl_order();
                od.o_fk_pro = item.productid;
                od.o_fk_invoice = iv.in_id;
                od.o_date = System.DateTime.Now;
                od.o_qty = item.qty;
                od.o_unitprice = (int)item.price;
                od.o_bill = item.bill;
                db.tbl_order.Add(od);
                db.SaveChanges();
            }

            TempData.Remove("total");
            TempData.Remove("cart");

            TempData["msg"] = "Transaction Complete...........";
            TempData.Keep();
            return RedirectToAction("Index");


        }

    }
}