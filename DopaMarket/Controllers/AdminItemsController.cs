﻿using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class AdminItemsController : BaseController
    {
        ApplicationDbContext _context;

        public AdminItemsController()
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
            viewModel.Brands = _context.Brands.Select(b => new SelectListItem() { Text = b.Name, Value = b.Id.ToString() }).ToArray();
            viewModel.Features = "";

            return View("ItemForm", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var item = _context.Items.SingleOrDefault<Item>(m => m.Id == id);
            if (item == null)
                return HttpNotFound();
        
            var viewModel = new ItemFormViewModel();
            viewModel.Item = item;
            viewModel.Categories = _context.Categories.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString()}).ToArray();
            viewModel.SelectedCategoryIds = _context.ItemCategories.Where(ic => ic.ItemId == id).Select(ic => ic.CategoryId).ToArray();
            viewModel.Brands = _context.Brands.Select(b => new SelectListItem() { Text = b.Name, Value = b.Id.ToString() }).ToArray();
            viewModel.Features = string.Join("\n", _context.ItemFeatures.Where(f => f.ItemId == id).Select(f => f.Text).ToArray());

            var keywordsList = (from ik in _context.ItemKeywords
                                join k in _context.Keywords on ik.KeywordId equals k.Id
                                where ik.ItemId == id
                                select k.Word).ToArray();

            viewModel.Keywords = string.Join(",", keywordsList);

            viewModel.ItemInfosData = SerializeItemInfos(item);

            return View("ItemForm", viewModel);
        }

        public string SerializeItemInfos(Item item)
        {
            var itemInfos = _context.ItemSpecifications.Where(ii => ii.ItemId == item.Id).Include(ii => ii.Specification);

            string result = "";
            foreach(var itemInfo in itemInfos)
            {
                result += itemInfo.Specification.Name + ":";
                switch (itemInfo.Specification.Type)
                {
                    case SpecificationType.Boolean:
                        result += itemInfo.BooleanValue;
                        break;
                    case SpecificationType.Interger:
                        result += itemInfo.IntegerValue;
                        break;
                    case SpecificationType.String:
                        result += itemInfo.StringValue;
                        break;
                    case SpecificationType.Decimal:
                        result += itemInfo.DecimalValue;
                        break;
                }
                result += "\n";
            }
            return result;
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
                itemInDB.ForSale = itemFormViewModel.Item.ForSale;
                itemInDB.CurrentPrice = itemFormViewModel.Item.CurrentPrice;
                itemInDB.InsertDate = itemFormViewModel.Item.InsertDate;
                itemInDB.BrandId = itemFormViewModel.Item.BrandId;
            }
            else
                _context.Items.Add(itemFormViewModel.Item);
        
            _context.SaveChanges();

            UpdateLinkItemCategories(itemFormViewModel.Item, itemFormViewModel.SelectedCategoryIds);
            UpdateLinkItemKeywords(itemFormViewModel.Item, itemFormViewModel.Keywords);

            UpdateItemImage(itemFormViewModel.Item, itemFormViewModel.UploadImages);
            UpdateItemInfos(itemFormViewModel.Item, itemFormViewModel.ItemInfosData);
            UpdateItemFeatures(itemFormViewModel.Item, itemFormViewModel.Features);

            return RedirectToAction("Index", "AdminItems");
        }
        
        public void UpdateLinkItemCategories(Item item, int[] categoriesId)
        {
            if(categoriesId == null)
            {
                categoriesId = new int[0];
            }

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

                item.ImageCount++;
                _context.SaveChanges();

                string imageName = item.LinkName + "_" + item.ImageCount + ".jpg";

                image.SaveAs(Server.MapPath("~\\Content\\ItemsImages") + "\\" + imageName);
            }
        }

        public void UpdateItemInfos(Item item, string itemInfosData)
        {
            if(itemInfosData == null)
                itemInfosData = "";

            var currentItemInfos = _context.ItemSpecifications.Where(ii => ii.ItemId == item.Id).ToList();
            _context.ItemSpecifications.RemoveRange(currentItemInfos);

            var itemInfoTypes = _context.Specifications.ToList();

            var itemInfoData = itemInfosData.Split('\n');
            foreach(var itemInfoSplited in itemInfoData)
            {
                var intemInfoType = itemInfoTypes.SingleOrDefault(iit => iit.Name == itemInfoSplited.Split(':')[0]);
                if (intemInfoType == null)
                    continue;

                var itemInfo = new ItemSpecification();
                itemInfo.ItemId = item.Id;
                itemInfo.SpecificationId = intemInfoType.Id;

                string value = itemInfoSplited.Split(':')[1];
                switch (intemInfoType.Type)
                {
                    case SpecificationType.Boolean:
                        itemInfo.BooleanValue = bool.Parse(value);
                        break;
                    case SpecificationType.Interger:
                        itemInfo.IntegerValue = int.Parse(value);
                        break;
                    case SpecificationType.String:
                        itemInfo.StringValue = value;
                        break;
                    case SpecificationType.Decimal:
                        itemInfo.DecimalValue = decimal.Parse(value);
                        break;
                }
                _context.ItemSpecifications.Add(itemInfo);
            }
            _context.SaveChanges();
        }

        void UpdateItemFeatures(Item item, string features)
        {
            var currentItemInfos = _context.ItemFeatures.Where(f => f.ItemId == item.Id).ToList();
            _context.ItemFeatures.RemoveRange(currentItemInfos);

            foreach(var featureText in features.Split('\n'))
            {
                var feature = new ItemFeature();
                feature.ItemId = item.Id;
                feature.Text = featureText;
                _context.ItemFeatures.Add(feature);
            }

            _context.SaveChanges();
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
        
            return RedirectToAction("Index", "AdminItems");
        }

        public ActionResult RemoveImage(int id)
        {
            //var itemImage = _context.ItemImages.SingleOrDefault(i => i.Id == id);
            //_context.ItemImages.Remove(itemImage);
            //_context.SaveChanges();
            //
            //System.IO.File.Delete(Server.MapPath("~\\Content\\ItemsImages") + "\\" + itemImage.Name);

            return Content("image removed");
        }
    }
}