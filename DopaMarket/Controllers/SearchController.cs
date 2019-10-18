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

        public ActionResult Index(string q, string c, decimal? minPrice = null, decimal? maxPrice = null, string b = "", string sort = "", int page = 0)
        {
            string keywordsString = q;
            string categoryLinkName = c;
            string brands = b;

            var searchRequest = new SearchTool.SearchRequest();

            if (brands != null && brands.Trim().Length > 0)
            {
                searchRequest.Brands = brands.Split(',').Select(s => s.Trim()).ToArray();
            }

            if (keywordsString != null)
            {
                searchRequest.Keywords = keywordsString.Split(',').Select(s => s.Trim()).ToArray();
            }

            if (categoryLinkName != null)
            {
                searchRequest.Category = _context.Categories.SingleOrDefault(cat => cat.LinkName == categoryLinkName);
            }

            searchRequest.Sort = sort != null ? sort : "a-z";
            searchRequest.Page = page;
            searchRequest.FilterPriceMin = minPrice != null ? (int)minPrice : 0;
            searchRequest.FilterPriceMax = maxPrice != null ? (int)maxPrice : 0;
            searchRequest.ItemPerPage = 3;

            var searchTool = new SearchTool(_context);
            var searchResult = searchTool.Execute(searchRequest);

            var searchViewModel = new SearchViewModel();
            searchViewModel.Query = keywordsString;
            searchViewModel.Category = categoryLinkName != null ? categoryLinkName : "";
            searchViewModel.Sort = sort;
            searchViewModel.PageNumber = page;
            searchViewModel.TotalCount = searchResult.ItemCount;
            searchViewModel.PageCount = searchResult.PageCount;
            searchViewModel.Categories = GetCategories();
            searchViewModel.PriceMin = searchResult.PriceMin;
            searchViewModel.PriceMax = searchResult.PriceMax;
            searchViewModel.PriceFilterMin = minPrice != null ? (int)minPrice : 0;
            searchViewModel.PriceFilterMax = maxPrice != null ? (int)maxPrice : 0;
            searchViewModel.Items = searchResult.Items;
            searchViewModel.Brands = new SearchBrandViewModel[0];// GetBrands(requestInfo.Brands);

            return View("Results", searchViewModel);
        }

        //SearchBrandViewModel[] GetBrands(SearchResult searchResult)
        //{
        //
        //}


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
                                                     .Select(c => new CategoryViewModel() { Category = c})
                                                     .ToArray();

                CategoryViewModels.Add(categoryViewModel);
            }

            return CategoryViewModels.ToArray();
        }
    }
}