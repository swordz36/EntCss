using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BabyStore.Web.DAL;
using BabyStore.Web.Models;
using System.Web.Helpers;
using BabyStore.Web.Helpers;
using BabyStore.Web.ViewModels;

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
        public async Task<ActionResult> Upload(HttpPostedFileBase[] files)
        {
            var allFilesValid = true;
            var invalidFiles = string.Empty;
            db.Database.Log = sql => Trace.WriteLine(sql);

            //chek is there is a file
            if (files[0] != null)
            {

                //if the user has entered less than ten files
                if (files.Length <= 10)
                {
                    //check they are all valid
                    foreach (var file in files)
                    {
                        if (!ValidateFile(file))
                        {
                            allFilesValid = false;
                            invalidFiles += ", " + file.FileName;
                        }
                    }

                    if (allFilesValid)
                    {
                        foreach (var file in files)
                        {
                            try
                            {
                                SaveFileToDisk(file);
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("FileName", "Sorry an error occurred saving the files to disk, please try again");
                            }
                        }
                    }
                    //else add an error listing out the invalid files
                    else
                    {
                        ModelState.AddModelError("FileName", $"All files must be gif, png, jpeg or jpg and less than 2MB in size.The following files {invalidFiles} are not valid");
                    }
                }
                //the user has entered more than 10 files
                else
                {
                    ModelState.AddModelError("FileName", "Please only upload up to ten files at a time");
                }
                //is valid file extention
            }
            else
            {
                //if the user has not entered a file return an error message
                ModelState.AddModelError("FileName", "Please choose a files");
            }

            if (ModelState.IsValid)
            {

                bool duplicates = false;
                bool otherDbError = false;
                string duplicateFiles = "";

                foreach (var file in files)
                {
                    //try and save each file
                    var productToAdd = new ProductImage {FileName = file.FileName};
                    try
                    {
                        db.ProductImages.Add(productToAdd);
                        db.SaveChanges();
                    }
                    //if there is an exception check if it is caused by a duplicate file
                    catch (DbUpdateException ex)
                    {
                        SqlException innerException = ex.InnerException.InnerException as SqlException;
                        if (innerException != null && innerException.Number == 2601)
                        {
                            duplicateFiles += ", " + file.FileName;
                            duplicates = true;
                            db.Entry(productToAdd).State = EntityState.Detached;
                        }
                        else
                        {
                            otherDbError = true;
                        }
                    }
                }
                //add a list of duplicate files to the error message
                if (duplicates)
                {
                    ModelState.AddModelError("FileName",
                        $"All files uploaded except the files {duplicateFiles}, which already exist in the system. Please delete them and try again if you wish to re - add them");
                    return View();
                }
                else if (otherDbError)
                {
                    ModelState.AddModelError("FileName",
                        "Sorry an error has occurred saving to the database, please try again");
                    return View();
                }

                return RedirectToAction("Index");
            }

            return View();
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
            System.IO.File.Delete(Request.MapPath(Constants.ImagePath + productImage.FileName));
            System.IO.File.Delete(Request.MapPath(Constants.ThumbnailImagePath + productImage.FileName));


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
