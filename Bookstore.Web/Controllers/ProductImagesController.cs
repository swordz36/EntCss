using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BabyStore.Web.DAL;
using BabyStore.Web.Models;
using System.Web.Helpers;
using BabyStore.Web.Helpers;

namespace BabyStore.Web.Controllers
{
    public class ProductImagesController : Controller
    {
        private StoreContext db = new StoreContext();

        // GET: ProductImages
        public async Task<ActionResult> Index()
        {
            return View(await db.ProductImages.ToListAsync());
        }

        // GET: ProductImages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = await db.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // GET: ProductImages/Create
        public ActionResult Upload()
        {
            return View();
        }

        // POST: ProductImages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            //chek is there is a file
            if (file != null)
            {
                //is valid file extention
                if(ValidateFile(file))
                {
                    try
                    {
                        SaveFileToDisk(file);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("FileName", "Sorry an error occurred saving the file to disk, please try again");
                    }
                }
                else
                {
                    ModelState.AddModelError("FileName", "The file must be gif, png, jpeg or jpg and less than 2MB in size");
                }
            }
            else
            {
                //if the user has not entered a file return an error message
                ModelState.AddModelError("FileName", "Please choose a file");
            }

            if (ModelState.IsValid)
            {
                db.ProductImages.Add(new ProductImage { FileName = file.FileName });
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: ProductImages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = await db.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // POST: ProductImages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FileName")] ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productImage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productImage);
        }

        // GET: ProductImages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = await db.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductImage productImage = await db.ProductImages.FindAsync(id);
            db.ProductImages.Remove(productImage);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ValidateFile(HttpPostedFileBase file)
        {
            string fileExtention = System.IO.Path.GetExtension(file.FileName).ToLower();
            string[] allowedFileTypes = { ".gif", ".png", ".jpeg", ".jpg" };

            if ((file.ContentLength > 0 && file.ContentLength < 2097152) && allowedFileTypes.Contains(fileExtention))
            {
                return true;
            }
            return false;
        }

        private void SaveFileToDisk(HttpPostedFileBase file)
        {
            var webImage = new WebImage(file.InputStream);

            if (webImage.Width > 190)
            {
                webImage.Resize(190, webImage.Height);
            }

            webImage.Save(Constants.ImagePath + file.FileName);
            if (webImage.Width > 100)
            {
                webImage.Resize(100, webImage.Height);
            }
            webImage.Save(Constants.ThumbnailImagePath + file.FileName);



        }
    }
}
