using ProductManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProductManagement.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Registration model)
        {
            using (var context = new RegisterEntities())
            {
                Registration registration = context.Registration.Add(model);
                context.SaveChanges();
            }
            return RedirectToAction("Login");
        }

        //Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.Login model)
        {
            using (var context = new RegisterEntities())
            {
                bool isvalid = context.Registration.Any(x => x.Email == model.Email && x.Password == model.Password);
                if (isvalid)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Page", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Data");
                    return View();
                }
            }

        }

        public ActionResult Page()
        {

            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        ProductsDBEntities db = new ProductsDBEntities();
        public ActionResult Index()
        {
            var data = db.Products.ToList();
            return View(data);
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Products p)
        {
            if (ModelState.IsValid==true)
            {
                string fileName = Path.GetFileNameWithoutExtension(p.ImageFile.FileName);
                string extention = Path.GetExtension(p.ImageFile.FileName);
                HttpPostedFileBase postedfile = p.ImageFile;
                int length = postedfile.ContentLength;

                if (extention.ToLower()==".jpg" || extention.ToLower() == ".jpeg" || extention.ToLower() == ".png")
                {
                    if (length <=1000000)
                    {
                        fileName = fileName + extention;
                        p.image_path = "~/images/"+fileName;
                        fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                        p.ImageFile.SaveAs(fileName);
                        db.Products.Add(p);
                        int a = db.SaveChanges();
                        if (a>0)
                        {
                            TempData["CreateMessage"] = "<script>alert('Data Added')</script>";
                            ModelState.Clear();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["CreateMessage"] = "<script>alert('Data not inserted')</script>";

                        }
                    
                    }
                    else
                    {
                        TempData["SizeMessage"] = "<script>alert('Image size should be less than 1MB')</script>";
                    }
                }
                else
                {
                    TempData["ExtensionMessage"] = "<script>alert('Format not supported')</script>";
                }

            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            var Productrow = db.Products.Where(model => model.ID == id).FirstOrDefault();
            Session["Image"] = Productrow.image_path;
            return View(Productrow);
        }

        [HttpPost]
        public ActionResult Edit(Products p)
        {
            if (ModelState.IsValid==true)
            {
                if (p.ImageFile !=null)
                {
                    
                        string fileName = Path.GetFileNameWithoutExtension(p.ImageFile.FileName);
                        string extention = Path.GetExtension(p.ImageFile.FileName);
                        HttpPostedFileBase postedfile = p.ImageFile;
                        int length = postedfile.ContentLength;

                        if (extention.ToLower() == ".jpg" || extention.ToLower() == ".jpeg" || extention.ToLower() == ".png")
                        {
                            if (length <= 1000000)
                            {
                                fileName = fileName + extention;
                                p.image_path = "~/images/" + fileName;
                                fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                                p.ImageFile.SaveAs(fileName);
                            db.Entry(p).State = EntityState.Modified;
                                int a = db.SaveChanges();
                                if (a > 0)
                                {
                                string ImagePath = Request.MapPath(Session["Image"].ToString());
                                if (System.IO.File.Exists(ImagePath))
                                {
                                    System.IO.File.Delete(ImagePath);
                                }
                                TempData["UpdateMessage"] = "<script>alert('Data Updated')</script>";
                                    ModelState.Clear();
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    TempData["UpdateMessage"] = "<script>alert('Data not Updated')</script>";

                                }

                            }
                            else
                            {
                                TempData["SizeMessage"] = "<script>alert('Image size should be less than 1MB')</script>";
                            }
                        }
                        else
                        {
                            TempData["ExtensionMessage"] = "<script>alert('Format not supported')</script>";
                        }

                    }
                else
                {
                    p.image_path = Session["Image"].ToString();
                    db.Entry(p).State = EntityState.Modified;
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["UpdateMessage"] = "<script>alert('Data Updated')</script>";
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "<script>alert('Data not Updated')</script>";

                    }
                }
                }
            return View();
            } 
        
          public ActionResult Delete(int id)
        {
            if (id>0)
            {
                var Productrow = db.Products.Where(model => model.ID == id).FirstOrDefault();
                if (Productrow != null)
                {
                    db.Entry(Productrow).State = EntityState.Deleted;
                    int a= db.SaveChanges();
                    if (a>0)
                    {
                        TempData["DeleteMessage"] = "<script>alert('Data Deleted')</script>";
                        string ImagePath = Request.MapPath(Productrow.image_path.ToString());
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }
                    else
                    {
                        TempData["DeleteMessage"] = "<script>alert('Data Not Deleted')</script>";
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }

        public ActionResult Details(int id)
        {
            var Productrow = db.Products.Where(model => model.ID == id).FirstOrDefault();
            Session["Image2"] = Productrow.image_path.ToString();
            return View(Productrow);
        }
            
        }
    }
