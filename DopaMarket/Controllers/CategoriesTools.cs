using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DopaMarket.Controllers
{
    public class CategoriesTools
    {
        public static Category[] GetBreadcrumb(ApplicationDbContext context, Category category)
        {
            var breadCrump = new List<Category>();
            if (category.ParentCategoryId != null)
            {
                var parentCategory = context.Categories.SingleOrDefault(c => c.Id == category.ParentCategoryId);
                var parentBreadcrumb = GetBreadcrumb(context, parentCategory);
                breadCrump.AddRange(parentBreadcrumb);
                breadCrump.Add(parentCategory);
            }
            return breadCrump.ToArray();
        }

        public static CategoryViewModel GetCategoryViewModel(ApplicationDbContext context, Category category)
        {
            var categoryViewModel = new CategoryViewModel();
            categoryViewModel.Category = category;
            categoryViewModel.Breadcrumb = GetBreadcrumb(context, category);
            return categoryViewModel;
        }
    }
}