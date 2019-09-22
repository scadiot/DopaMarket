using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class CategoriesController : Controller
    {
        ApplicationDbContext _context;

        public CategoriesController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View("Index", categories);
        }

        public ActionResult Edit(int id)
        {
            var category = _context.Categories.SingleOrDefault<Category>(c => c.Id == id);
            if (category == null)
                return HttpNotFound();

            var viewModel = new CategoryFormViewModel();
            viewModel.Breadcrumb = CategoriesTools.GetBreadcrumb(_context, category);
            viewModel.Category = category;

            return View("CategoryForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(Category category)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CategoryFormViewModel();
                viewModel.Breadcrumb = CategoriesTools.GetBreadcrumb(_context, category);
                viewModel.Category = category;
                return View("CategoryForm", viewModel);
            }

            if (category.Id != 0)
            {
                var categoryInDB = _context.Categories.Single<Category>(c => c.Id == category.Id);
                categoryInDB.Name = category.Name;
            }
            else
                _context.Categories.Add(category);

            _context.SaveChanges();

            return RedirectToAction("Index", "Categories");
        }

        public ActionResult Delete(int id)
        {
            var category = _context.Categories.Single<Category>(c => c.Id == id);

            var linksToRemove = _context.ItemCategories.Where(ic => ic.CategoryId == id);
            _context.ItemCategories.RemoveRange(linksToRemove);

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction("Index", "Categories");
        }
    }
}