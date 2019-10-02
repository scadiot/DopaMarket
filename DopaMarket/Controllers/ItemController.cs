using DopaMarket.Models;
using DopaMarket.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class ItemController : BaseController
    {
        ApplicationDbContext _context;

        public ItemController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Item
        public ActionResult Detail(string linkName)
        {
            var item = _context.Items.SingleOrDefault(i => i.LinkName == linkName);
            if (item == null)
            {
                return View("Detail");
            }

            var itemDetailModel = new ItemDetailModel();
            itemDetailModel.Item = item;
            itemDetailModel.ItemInfos = getItemDetailItemInfoModel(item);

            itemDetailModel.Reviews = _context.ItemReviews
                                              .Where(ir => ir.ItemId == item.Id)
                                              .Include(ir => ir.Customer)
                                              .OrderBy(ir => ir.Date)
                                              .ToArray();

            itemDetailModel.Features = _context.ItemFeatures
                                               .Where(i => i.ItemId == item.Id)
                                               .ToArray();

            itemDetailModel.Images = _context.ItemImages
                                             .Where(i => i.ItemId == item.Id)
                                             .ToArray();

            var starInfos = new List<StarInfo>();
            for(int i = 1;i < 6;i++)
            {
                var starInfo = new StarInfo();
                starInfo.Value = i;
                starInfo.Count = itemDetailModel.Reviews.Count(r => r.Note == i);
                starInfo.Ratio = itemDetailModel.Reviews.Count() > 0 ? (decimal)starInfo.Count / itemDetailModel.Reviews.Count() : 0;
                starInfos.Add(starInfo);
            }
            itemDetailModel.Stars = starInfos.ToArray();

            return View("Detail", itemDetailModel);
        }

        ItemDetailItemInfoModel[] getItemDetailItemInfoModel(Item item)
        {
            var itemInfos = _context.ItemInfos
                                    .Where(ii => ii.ItemId == item.Id)
                                    .Include(ii => ii.ItemInfoType)
                                    .ToArray();

            var result = new List<ItemDetailItemInfoModel>();
            foreach (var itemInfo in itemInfos)
            {
                var ItemDetailItemInfoModel = new ItemDetailItemInfoModel();
                ItemDetailItemInfoModel.Name = itemInfo.ItemInfoType.LongName;
                switch (itemInfo.ItemInfoType.Type)
                {
                    case ItemInfoValueType.Boolean:
                        ItemDetailItemInfoModel.Value = itemInfo.BooleanValue.ToString();
                        break;
                    case ItemInfoValueType.Interger:
                        ItemDetailItemInfoModel.Value = itemInfo.IntegerValue.ToString();
                        break;
                    case ItemInfoValueType.String:
                        ItemDetailItemInfoModel.Value = itemInfo.StringValue;
                        break;
                    case ItemInfoValueType.Decimal:
                        ItemDetailItemInfoModel.Value = itemInfo.DecimalValue.ToString();
                        break;
                }
                
                ItemDetailItemInfoModel.Unity = itemInfo.ItemInfoType.Unity;
                result.Add(ItemDetailItemInfoModel);
            }
            return result.ToArray();
        }

        [HttpPost]
        public ActionResult PushReview(int itemId, int rating, string text)
        {
            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == userId);
            var item = _context.Items.SingleOrDefault(i => i.Id == itemId);

            var itemReview = _context.ItemReviews.SingleOrDefault(ir => ir.ItemId == itemId && ir.CustomerId == customer.Id);
            if(itemReview == null)
            {
                itemReview = new ItemReview();
                itemReview.CustomerId = customer.Id;
                itemReview.ItemId = itemId;
                itemReview.Note = rating;
                itemReview.Text = text;
                itemReview.Date = DateTime.Now;
                _context.ItemReviews.Add(itemReview);
            }
            else
            {
                itemReview.Note = rating;
                itemReview.Text = text;
                itemReview.Date = DateTime.Now;
            }
            _context.SaveChanges();
            return RedirectToAction(item.LinkName, "Item");
        }
    }
}