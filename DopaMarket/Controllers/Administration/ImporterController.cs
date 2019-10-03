using DopaMarket.Models;
using DopaMarket.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DopaMarket.Controllers.Administration
{
    public class ImporterController : BaseController
    {
        ApplicationDbContext _context;
        public ImporterController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Importer
        public ActionResult Index()
        {
            var result = new ImporterViewModel();
            return View("Index", result);
        }

        [HttpPost]
        public ActionResult Save(string data)
        {
            Data parsedData = JsonConvert.DeserializeObject<Data>(data);

            ImportCustomers(parsedData);
            ImportCategories(parsedData);
            ImportSpecifications(parsedData);
            ImportCompareGroups(parsedData);
            ImportCompareGroupSpecifications(parsedData);
            ImportItems(parsedData);

            var result = new ImporterViewModel();
            result.Data = data;
            result.Error = "";
            return View("Index", data);
        }

        void ImportCustomers(Data data)
        {
            //_context.Customers
        }

        void ImportCategories(Data data)
        {
            //_context.Customers
        }

        void ImportSpecifications(Data data)
        {
            //_context.Customers
        }

        void ImportCompareGroups(Data data)
        {
            //_context.Customers
        }

        void ImportCompareGroupSpecifications(Data data)
        {
            //_context.Customers
        }

        void ImportItems(Data data)
        {
            //_context.Customers
        }

        public class Customer
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string PostalCode { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
        }

        public class Categorie
        {
            public string Name { get; set; }
            public string LinkName { get; set; }
            public string Parent { get; set; }
        }

        public class Specification
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Unity { get; set; }
            public string LongName { get; set; }
        }

        public class CompareGroup
        {
            public string Name { get; set; }
            public string LinkName { get; set; }
        }

        public class CompareGroupSpecification
        {
            public string CompareGroup { get; set; }
            public string Specification { get; set; }
        }

        public class Review
        {
            public int Rate { get; set; }
            public string title { get; set; }
            public string Text { get; set; }
            public string Email { get; set; }
        }

        public class ItemSpecification
        {
            public string specification { get; set; }
            public int IntegerValue { get; set; }
            public decimal DecimalValue { get; set; }
            public bool BooleanValue { get; set; }
            public string StringValue { get; set; }
        }

        public class Item
        {
            public string Name { get; set; }
            public string LinkName { get; set; }
            public string InsertDate { get; set; }
			public string TinyDescriptive { get; set; }
			public string Descriptive { get; set; }
            public decimal CurrentPrice { get; set; }
			public string Brand { get; set; }
            public string MainCategory { get; set; }
			public decimal Weight { get; set; }
			public decimal Width { get; set; }
			public decimal Height { get; set; }
			public decimal Length { get; set; }
            public IList<Review> Reviews { get; set; }
            public IList<string> Categories { get; set; }
            public IList<string> Keywords { get; set; }
            public IList<string> Features { get; set; }
            public IList<ItemSpecification> ItemSpecifications { get; set; }
    }

        public class Data
        {
            public IList<Customer> Customers { get; set; }
            public IList<Categorie> Categories { get; set; }
            public IList<Specification> Specifications { get; set; }
            public IList<CompareGroup> CompareGroups { get; set; }
            public IList<CompareGroupSpecification> CompareGroupSpecifications { get; set; }
            public IList<Item> Items { get; set; }
        }
    }
}