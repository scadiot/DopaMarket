using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers
{
    public class ItemController : Controller
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
    }
}