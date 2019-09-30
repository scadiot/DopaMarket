using DopaMarket.Models;
using DopaMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers.Administration
{
    public class BrandController : Controller
    {
        ApplicationDbContext _context;

        public BrandController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var brands = _context.Brands.ToList();
            return View("Index", brands);
        }

        public ActionResult Edit(int id)
        {
            var brand = _context.Brands.SingleOrDefault<Brand>(c => c.Id == id);
            if (brand == null)
                return HttpNotFound();

            var viewModel = new BrandFormViewModel();
            viewModel.Brand = brand;

            return View("CategoryForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new BrandFormViewModel();
                viewModel.Brand = brand;
                return View("CategoryForm", viewModel);
            }

            if (brand.Id != 0)
            {
                var brandInDB = _context.Brands.Single<Brand>(c => c.Id == brand.Id);
                brandInDB.Name = brand.Name;
                brandInDB.LinkName = brand.LinkName;
            }
            else
                _context.Brands.Add(brand);

            _context.SaveChanges();

            return RedirectToAction("Index", "Brands");
        }

        public ActionResult Delete(int id)
        {
            var brand = _context.Brands.Single<Brand>(c => c.Id == id);


            return RedirectToAction("Index", "Brands");
        }
    }
}