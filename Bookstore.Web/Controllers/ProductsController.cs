using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using X.PagedList;
using BabyStore.Web.DAL;
using BabyStore.Web.Models;
using BabyStore.Web.ViewModels;
using BabyStore.Web.Helpers;

namespace BabyStore.Web.Controllers
{
    public class ProductsController : Controller
    {
        private StoreContext db = new StoreContext();

        // GET: Products
        public ActionResult Index(string category, string search, string sortBy, int? page)
        {
            var viewModel = new ProductIndexViewModel();

            var products = db.Products.Include(p => p.Category);

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(x => x.Name.Contains(search)
                || x.Description.Contains(search)
                || x.Category.Name.Contains(search));
                viewModel.Search = search;
            }

            //group search results into categories and count how many items in each category
            viewModel.CatWithCount =
                products.Where(matchingProducts => matchingProducts.CategoryId != null)
                    .GroupBy(matchingProducts => matchingProducts.Category.Name)
                    .Select(catGroup => new CategoryWithCount()
                    {
                        CategoryName = catGroup.Key,
                        ProductCount = catGroup.Count()
                    });

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.Name == category);
                viewModel.Category = category;
            }

            //sort the results
            switch (sortBy)
            {
                case "price_lowest":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_highest":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            int currentPage = (page ?? 1);
            viewModel.Products = products.ToPagedList(currentPage, Constants.PageItems);
            viewModel.SortBy = sortBy;

            viewModel.Sorts = new Dictionary<string, string>
            {
                {"Price low to high", "price_lowest" },
                {"Price high to low", "price_highest" }
            };

            return View(viewModel);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var viewModel = new ProductViewModel();
            viewModel.CategoryList = new SelectList(db.Categories, "Id","Name");
            viewModel.ImageLists = new List<SelectList>();

            for (int i = 0; i < Constants.NumberOfProductImages; i++)
            {
                viewModel.ImageLists.Add(new SelectList(db.ProductImages,"Id","FileName"));
            }

            return View(viewModel);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductViewModel viewModel)
        {
            Product product = new Product();
            product.Name = viewModel.Name;
            product.Description = viewModel.Description;
            product.Price = viewModel.Price;
            product.CategoryId = viewModel.CategoryId;

            product.ProductImageMappings = new List<ProductImageMapping>();

            //get a list of selected images without any blanks
           var productImages = viewModel.ProductImages.Where(productImage => !string.IsNullOrEmpty(productImage)).ToArray();

            for (int i = 0; i < productImages.Length; i++)
            {
                product.ProductImageMappings.Add(new ProductImageMapping
                {
                    ProductImage = db.ProductImages.Find(int.Parse(productImages[i])),
                    ImageNumber = i
                });
            }

            viewModel.CategoryList = new SelectList(db.Categories, "Id", "Name", product.CategoryId);
            viewModel.ImageLists = new List<SelectList>();

            for (int i = 0; i < Constants.NumberOfProductImages; i++)
            {
                viewModel.ImageLists.Add(new SelectList(db.ProductImages, "ID", "FileName",viewModel.ProductImages[i]));
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

           
            return View(viewModel);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ProductViewModel();
            viewModel.CategoryList = new SelectList(db.Categories, "ID", "Name", product.CategoryId);
            viewModel.ImageLists = new List<SelectList>();

            foreach (var imageMapping in product.ProductImageMappings.OrderBy(x=>x.ImageNumber))
            {
                viewModel.ImageLists.Add(new SelectList(db.ProductImages,"Id","FileName",imageMapping.ProductImageId));
            }

            for (int i = viewModel.ImageLists.Count; i < Constants.NumberOfProductImages; i++)
            {
                viewModel.ImageLists.Add(new SelectList(db.ProductImages,"Id","FileName"));   
            }

            viewModel.Id = product.Id;
            viewModel.Name = product.Name;
            viewModel.Description = product.Description;
            if (product.Price != null) viewModel.Price = product.Price.Value;
            return View(viewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductViewModel viewModel)
        {
            var productToUpdate = db.Products.Include(p => p.ProductImageMappings).Single(x => x.Id == viewModel.Id);

            if (TryUpdateModel(productToUpdate,string.Empty,new string[] {"Name","Description","Price","CategoryId"}))
            {
                if (productToUpdate.ProductImageMappings == null)
                {
                    productToUpdate.ProductImageMappings = new List<ProductImageMapping>();

                    //get a list of selected images without any blanks
                    string[] productImages = viewModel.ProductImages.Where(pi =>!string.IsNullOrEmpty(pi)).ToArray();
                    for (int i = 0; i < productImages.Length; i++)
                    {
                        //get image currently stored
                        var imageMappingToEdit = productToUpdate.ProductImageMappings.FirstOrDefault(x => x.ImageNumber == i);
                        var image = await db.ProductImages.FindAsync(int.Parse(productImages[i]));

                        //if there is nothing to be stored then we need ot add a new mapping
                        if (imageMappingToEdit == null)
                        {
                            //add Image to ImageMappings
                            productToUpdate.ProductImageMappings.Add(new ProductImageMapping
                            {
                                ImageNumber = i,
                                ProductImage = image,
                                ProductImageId = image.Id

                            });   
                        }
                        else
                        {
                            //if they are not the same 
                            if (imageMappingToEdit.ProductImageId != int.Parse(productImages[i]))
                            {
                                //assign imageProperty of the image
                                imageMappingToEdit.ProductImage = image;
                            }
                        }
                    }

                    for (int i = productImages.Length; i < Constants.NumberOfProductImages; i++)
                    {
                        var imageMappingToEdit = productToUpdate.ProductImageMappings.FirstOrDefault(x => x.ImageNumber == i);

                        if (imageMappingToEdit != null)
                        {
                            //delete the record from the mapping table directly.
                            //just calling productToUpdate.ProductImageMappings.Remove(imageMappingToEdit)
                            //results in a FK error

                            db.ProdcutImageMappings.Remove(imageMappingToEdit);

                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(viewModel);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
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
    }
}
