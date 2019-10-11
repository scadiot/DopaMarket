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
            itemDetailModel.ItemSpecifications = getItemSpecificationsModel(item);

            itemDetailModel.Reviews = _context.ItemReviews
                                              .Where(ir => ir.ItemId == item.Id)
                                              .Include(ir => ir.Customer)
                                              .Include(ir => ir.Customer.ApplicationUser)
                                              .OrderBy(ir => ir.Date)
                                              .ToArray();

            itemDetailModel.Features = _context.ItemFeatures
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
            itemDetailModel.OtherItems = _context.ItemLinks
                                                 .Where(i => i.MainItemId == item.Id)
                                                 .Include(i => i.OtherItem)
                                                 .Select(i => i.OtherItem)
                                                 .ToArray();

            return View("Detail", itemDetailModel);
        }

        ItemSpecificationModel[] getItemSpecificationsModel(Item item)
        {
            var itemInfos = _context.ItemSpecifications
                                    .Where(ii => ii.ItemId == item.Id)
                                    .Include(ii => ii.Specification)
                                    .ToArray();

            var result = new List<ItemSpecificationModel>();
            foreach (var itemInfo in itemInfos)
            {
                var ItemItemSpecificationModel = new ItemSpecificationModel();
                ItemItemSpecificationModel.Name = itemInfo.Specification.LongName;
                switch (itemInfo.Specification.Type)
                {
                    case SpecificationType.Boolean:
                        ItemItemSpecificationModel.Value = itemInfo.BooleanValue.ToString();
                        break;
                    case SpecificationType.Interger:
                        ItemItemSpecificationModel.Value = itemInfo.IntegerValue.ToString();
                        break;
                    case SpecificationType.String:
                        ItemItemSpecificationModel.Value = itemInfo.StringValue;
                        break;
                    case SpecificationType.Decimal:
                        ItemItemSpecificationModel.Value = itemInfo.DecimalValue.ToString();
                        break;
                }
                
                ItemItemSpecificationModel.Unity = itemInfo.Specification.Unity;
                result.Add(ItemItemSpecificationModel);
            }
            return result.ToArray();
        }

        [HttpPost]
        public ActionResult PushReview(int itemId, string title, int rating, string text)
        {
            var userId = User.Identity.GetUserId().ToString();
            var customer = _context.Customers.SingleOrDefault(c => c.ApplicationUserId == userId);
            var item = _context.Items.SingleOrDefault(i => i.Id == itemId);

            var itemReview = _context.ItemReviews.SingleOrDefault(ir => ir.ItemId == itemId && ir.CustomerId == customer.Id);
            if(itemReview == null)
            {
                itemReview = new ItemReview();
                itemReview.CustomerId = customer.Id;
                itemReview.ItemId = itemId;
                itemReview.Title = title;
                itemReview.Note = rating;
                itemReview.Text = text;
                itemReview.Date = DateTime.Now;
                _context.ItemReviews.Add(itemReview);
            }
            else
            {
                itemReview.Note = rating;
                itemReview.Text = text;
                itemReview.Title = title;
                itemReview.Date = DateTime.Now;
            }
            _context.SaveChanges();

            var rates = _context.ItemReviews.Where(r => r.ItemId == item.Id).Select(r => r.Note).ToArray();
            item.AverageRating = (decimal)rates.Sum() / rates.Count();
            _context.SaveChanges();

            return RedirectToAction(item.LinkName, "Item");
        }
    }
}