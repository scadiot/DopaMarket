using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class ItemsController : Controller
    {
        ApplicationDbContext _context;

        public ItemsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var items = _context.Items.ToList();
            return View("Index", items);
        }

        public ActionResult New()
        {
            var viewModel = new ItemFormViewModel();
            viewModel.Item = new Item();
            viewModel.Item.InsertDate = DateTime.Now;
            viewModel.Categories = _context.Categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.ToString() }).ToList();
            viewModel.Keywords = "";
            viewModel.SelectedCategoryIds = new int[0];

            return View("ItemForm", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var item = _context.Items.SingleOrDefault<Item>(m => m.Id == id);
            if (item == null)
                return HttpNotFound();
        
            var viewModel = new ItemFormViewModel();
            viewModel.Item = item;
            viewModel.Categories = _context.Categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString()}).ToList();
            viewModel.SelectedCategoryIds = _context.ItemCategories.Where(ic => ic.ItemId == id).Select(ic => ic.CategoryId).ToArray();

            var keywordsList = (from ik in _context.ItemKeywords
                                join k in _context.Keywords on ik.KeywordId equals k.Id
                                where ik.ItemId == id
                                select k.Word).ToArray();

            viewModel.Keywords = string.Join(",", keywordsList);

            return View("ItemForm", viewModel);
        }
        
        [HttpPost]
        public ActionResult Save(ItemFormViewModel itemFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("ItemForm", itemFormViewModel);
            }
        
            if (itemFormViewModel.Item.Id != 0)
            {
                var itemInDB = _context.Items.Single<Item>(m => m.Id == itemFormViewModel.Item.Id);
                itemInDB.Name = itemFormViewModel.Item.Name;
                itemInDB.OnSale = itemFormViewModel.Item.OnSale;
                itemInDB.InsertDate = itemFormViewModel.Item.InsertDate;
            }
            else
                _context.Items.Add(itemFormViewModel.Item);
        
            _context.SaveChanges();

            UpdateLinkItemCategories(itemFormViewModel.Item, itemFormViewModel.SelectedCategoryIds);

            var keywords = itemFormViewModel.Keywords.Split(',').ToList().Select(k => k.Trim().ToLower()).ToArray();
            UpdateLinkItemKeywords(itemFormViewModel.Item, keywords);

            var file = Request.Files[0];

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                file.SaveAs(path);
            }

            return RedirectToAction("Index", "Items");
        }
        
        public void UpdateLinkItemCategories(Item item, int[] categoriesId)
        {
            var linkToRemove = _context.ItemCategories.Where(ic => ic.ItemId == item.Id && !categoriesId.Contains(ic.CategoryId));
            _context.ItemCategories.RemoveRange(linkToRemove);

            var existingLinks = _context.ItemCategories.Where(ic => ic.ItemId == item.Id).Select(ic => ic.CategoryId).ToArray();

            foreach (var categoryId in categoriesId)
            {
                if (!existingLinks.Contains(categoryId))
                {
                    var itemCategoryLink = new ItemCategory();
                    itemCategoryLink.CategoryId = categoryId;
                    itemCategoryLink.ItemId = item.Id;
                    _context.ItemCategories.Add(itemCategoryLink);
                }
            }
            _context.SaveChanges();
        }

        public void UpdateLinkItemKeywords(Item item, string[] keywords)
        {
            foreach(var keyword in keywords)
            {
                if (!_context.Keywords.Any(k => k.Word == keyword))
                {
                    var newKeyword = new Keyword()
                    {
                        Word = keyword
                    };
                    _context.Keywords.Add(newKeyword);
                }
            }
            _context.SaveChanges();

            var keywordIds = _context.Keywords.Where(k => keywords.Contains(k.Word)).Select(k => k.Id).ToList();

            var linkToRemove = _context.ItemKeywords.Where(ik => ik.ItemId == item.Id && !keywordIds.Contains(ik.KeywordId));
            _context.ItemKeywords.RemoveRange(linkToRemove);

            var existingLinks = _context.ItemKeywords.Where(ic => ic.ItemId == item.Id).Select(ic => ic.KeywordId).ToArray();

            foreach (var keywordId in keywordIds)
            {
                if (!existingLinks.Contains(keywordId))
                {
                    var itemKeywordLink = new ItemKeyword();
                    itemKeywordLink.KeywordId = keywordId;
                    itemKeywordLink.ItemId = item.Id;
                    _context.ItemKeywords.Add(itemKeywordLink);
                }
            }
            _context.SaveChanges();
        }

        public ActionResult Delete(int id)
        {
            var category = _context.Items.Single<Item>(c => c.Id == id);
        
            _context.Items.Remove(category);
            _context.SaveChanges();
        
            return RedirectToAction("Index", "Items");
        }
    }
}