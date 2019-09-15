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
            viewModel.Categories = _context.Categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString() }).ToList();
            viewModel.Keywords = "";
            viewModel.SelectedCategoryIds = new int[0];
            viewModel.Images = new ItemImage[0];

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
            viewModel.Images = _context.ItemImages.Where(ii => ii.ItemId == id).ToList();

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
                itemInDB.LinkName = itemFormViewModel.Item.LinkName;
                itemInDB.OnSale = itemFormViewModel.Item.OnSale;
                itemInDB.CurrentPrice = itemFormViewModel.Item.CurrentPrice;
                itemInDB.InsertDate = itemFormViewModel.Item.InsertDate;
            }
            else
                _context.Items.Add(itemFormViewModel.Item);
        
            _context.SaveChanges();

            UpdateLinkItemCategories(itemFormViewModel.Item, itemFormViewModel.SelectedCategoryIds);
            UpdateLinkItemKeywords(itemFormViewModel.Item, itemFormViewModel.Keywords);

            UpdateItemImage(itemFormViewModel.Item, itemFormViewModel.UploadImages);

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

        public void UpdateLinkItemKeywords(Item item, string keywordsData)
        {
            if(keywordsData == null)
            {
                return;
            }

            var keywords = keywordsData.Split(',').ToList().Select(k => k.Trim().ToLower()).ToArray();

            foreach (var keyword in keywords)
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

        public void UpdateItemImage(Item item, HttpPostedFileBase[] uploadImages)
        {
            foreach(var image in uploadImages)
            {
                if(image == null)
                {
                    continue;
                }

                var itemImage = new ItemImage();
                itemImage.ItemId = item.Id;
                _context.ItemImages.Add(itemImage);
                _context.SaveChanges();

                itemImage.Name = item.LinkName + "_" + itemImage.Id + ".jpg";
                itemImage.Path = "\\Content\\ItemsImages\\" + itemImage.Name;
                _context.SaveChanges();

                image.SaveAs(Server.MapPath("~\\Content\\ItemsImages") + "\\" + itemImage.Name);
            }
        }

        public ActionResult Delete(int id)
        {
            var keywordLinksToRemove = _context.ItemKeywords.Where(ik => ik.ItemId == id);
            _context.ItemKeywords.RemoveRange(keywordLinksToRemove);

            var categoryLinksToRemove = _context.ItemCategories.Where(ic => ic.ItemId == id);
            _context.ItemCategories.RemoveRange(categoryLinksToRemove);

            var item = _context.Items.Single<Item>(c => c.Id == id);
            _context.Items.Remove(item);

            _context.SaveChanges();
        
            return RedirectToAction("Index", "Items");
        }

        public ActionResult RemoveImage(int id)
        {
            var itemImage = _context.ItemImages.SingleOrDefault(i => i.Id == id);
            _context.ItemImages.Remove(itemImage);
            _context.SaveChanges();

            System.IO.File.Delete(Server.MapPath("~\\Content\\ItemsImages") + "\\" + itemImage.Name);

            return Content("image removed");
        } 
    }
}