using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBlog.Models;
using System.Web.Helpers;
using System.IO;
using PagedList;
using PagedList.Mvc;
using System.Net;

namespace MvcBlog.Controllers
{
    public class AdminMakaleController : Controller
    {
        mvcblogDB db = new mvcblogDB();

        // GET: AdminMakale
        public ActionResult Index(int Page=1 )
        {
            var makales = db.Makales.OrderByDescending(m => m.MakaleId).ToPagedList(Page,10);
            return View(makales);
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Makale makale = db.Makales.Find(id);
            if (makale == null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.KategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale, string etiketler, HttpPostedFileBase Foto)
        {
            try
            {
                if (Foto != null)
                {
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);

                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makale.Foto= "/Uploads/MakaleFoto/" + newfoto;

                    
                    
                }
                if (etiketler!=null)
                {
                    string[] etiketDizi = etiketler.Split(',');
                    foreach (var i in etiketDizi)
                    {
                        var yeniEtiket = new Etiket { EtiketAdi = i };
                        db.Etikets.Add(yeniEtiket);
                        makale.Etikets.Add(yeniEtiket);
                    }
                }
                makale.UyeId = Convert.ToInt32(Session["uyeid"]);
                makale.Okunma = 1;
                db.Makales.Add(makale);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(makale);
            }
        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            ViewBag.kategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Foto, Makale makale)
        {
            try
            {
                var makales = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (Foto != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(makales.Foto));
                    }
                    WebImage img = new WebImage(Foto.InputStream);
                    FileInfo fotoinfo = new FileInfo(Foto.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(800, 350);
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makales.Foto = "/Uploads/MakaleFoto/" + newfoto;                    
                }
                makales.Baslik = makale.Baslik;
                makales.Icerik = makale.Icerik;
                makales.KategoriId = makale.KategoriId;
                makales.Tarih = makale.Tarih;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }

            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var makales = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (makales == null)
                {
                    return HttpNotFound();
                }

                if (System.IO.File.Exists(Server.MapPath(makales.Foto)))
                {
                    System.IO.File.Delete(Server.MapPath(makales.Foto));
                }

                foreach (var i in makales.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var  i in makales.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(makales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
