using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using PagedList;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        dbemarketingEntities1 db = new dbemarketingEntities1();

        
        // GET: Admin
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult login(tbl_admin avm)
        {
            tbl_admin ad = db.tbl_admin.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();
            if (ad != null)
            {

                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("ViewCategory");

            }
            else
            {
                ViewBag.error = "Invalid username or password";

            }

            return View();
        }

        public ActionResult userlist()
        {
            List<tbl_user> us = db.tbl_user.ToList();

            return View(us);
        }

        public ActionResult Details(int id)
        {

            var product = " select p.* from tbl_user w right join tbl_product p on w.u_id = p.pro_fk_user ";
            List<tbl_product> us2 = db.tbl_product.SqlQuery(product).ToList();

            return View(us2);



        }

        public ActionResult AdsCat(int? id, int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }

        [HttpPost]
        public ActionResult AdsCat(int? id, int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);


        }




        public ActionResult Create()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("login");
            }
            return View();
        }


        [HttpPost]
        public ActionResult Create(tbl_category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_category cat = new tbl_category();
                cat.cat_name = cvm.cat_name;
                cat.cat_image = path;
                cat.cat_status = 1;
                cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                db.tbl_category.Add(cat);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }

            return View();
        } //end,,,,,,,,,,,,,,,,,,,



        public ActionResult orderlist()
        {
            List<tbl_invoice> us = db.tbl_invoice.ToList();

            return View(us);
        }


        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);




        }

        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }
        public ActionResult ViewAdCat(int? id)
        {
            Adviewmodel ad = new Adviewmodel();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_name;
            ad.pro_image = p.pro_image;
            ad.pro_price = p.pro_price;
            ad.pro_des = p.pro_des;
            tbl_category cat = db.tbl_category.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ad.u_name = u.u_name;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;
            ad.pro_fk_user = u.u_id;




            return View(ad);
        }


        public ActionResult Delete(int? id)
        {

            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            db.tbl_product.Remove(p);
            db.SaveChanges();

            return RedirectToAction("ViewCategory");


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








    }
}