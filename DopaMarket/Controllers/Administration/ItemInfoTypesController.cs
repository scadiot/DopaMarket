using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers.Administration
{
    public class ItemInfoTypesController : BaseController
    {
        ApplicationDbContext _context;

        public ItemInfoTypesController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var itemInfoTypes = _context.Specifications.ToList();
            return View("Index", itemInfoTypes);
        }

        public ActionResult New()
        {
            var viewModel = new ItemInfoTypesFormViewModel();
            viewModel.ItemInfoType = new Specification();
            viewModel.ItemInfoType.Type = SpecificationType.String;

            return View("ItemInfoTypeForm", viewModel);
        }

        public ActionResult Edit(int id)
        {
            var itemInfoTypes = _context.Specifications.SingleOrDefault<Specification>(c => c.Id == id);
            if (itemInfoTypes == null)
                return HttpNotFound();

            var viewModel = new ItemInfoTypesFormViewModel();
            viewModel.ItemInfoType = itemInfoTypes;

            return View("ItemInfoTypeForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(Specification itemInfoType)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new ItemInfoTypesFormViewModel();
                viewModel.ItemInfoType = itemInfoType;
                return View("ItemInfoTypeForm", viewModel);
            }

            if (itemInfoType.Id != 0)
            {
                var itemInfoTypeInDB = _context.Specifications.Single<Specification>(c => c.Id == itemInfoType.Id);
                itemInfoTypeInDB.Name = itemInfoType.Name;
                itemInfoTypeInDB.LongName = itemInfoType.LongName;
                itemInfoTypeInDB.Unity = itemInfoType.Unity;
            }
            else
                _context.Specifications.Add(itemInfoType);

            _context.SaveChanges();

            return RedirectToAction("Index", "ItemInfoTypes");
        }

        public ActionResult Delete(int id)
        {
            var itemInfoType = _context.Specifications.Single<Specification>(c => c.Id == id);

            var itemInfosToRemove = _context.ItemSpecifications.Where(ic => ic.ItemInfoTypeId == id);
            _context.ItemSpecifications.RemoveRange(itemInfosToRemove);

            _context.Specifications.Remove(itemInfoType);
            _context.SaveChanges();

            return RedirectToAction("Index", "ItemInfoTypes");
        }
    }
}