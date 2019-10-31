using DopaMarket.Models;
using DopaMarket.ViewModels;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class SearchController : BaseController
    {
        const int ItemPerPage = 3;

        ApplicationDbContext _context;

        public SearchController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var searchViewModel = new SearchViewModel();
            searchViewModel.Categories = GetCategories();

            return View("Index", searchViewModel);
        }

        CategoryViewModel[] GetCategories()
        {
            var categories = _context.Categories.Where(c => c.ParentCategoryId == null).ToArray();
            var categoriesIds = categories.Select(cat => cat.Id).ToArray();
            var children = _context.Categories.Where(c => categoriesIds.Contains((int)c.ParentCategoryId)).ToArray();

            var CategoryViewModels = new List<CategoryViewModel>();
            foreach (var category in categories)
            {
                var categoryViewModel = new CategoryViewModel();
                categoryViewModel.Category = category;

                categoryViewModel.Children = children.Where(c => c.ParentCategoryId == category.Id)
                                                     .Select(c => new CategoryViewModel() { Category = c })
                                                     .ToArray();

                CategoryViewModels.Add(categoryViewModel);
            }

            return CategoryViewModels.ToArray();
        }
    }
}