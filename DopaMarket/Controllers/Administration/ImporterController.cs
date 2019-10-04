using DopaMarket.Models;
using DopaMarket.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            foreach (var customerData in data.Customers)
            {
                var applicationUser = _context.Users.SingleOrDefault(u => u.Email == customerData.Email);
                if (applicationUser == null)
                {
                    applicationUser = new ApplicationUser { UserName = customerData.Email, Email = customerData.Email };
                    var task = userManager.CreateAsync(applicationUser, customerData.Password);
                    if(task.Status == TaskStatus.Running)
                    {
                        task.RunSynchronously();
                    }
                    var result = task.Result;
                    if (!result.Succeeded)
                    {
                        continue;
                    }
                }

                var customer = _context.Customers.SingleOrDefault(c => c.IdentityUserId == applicationUser.Id);
                if (customer == null)
                {
                    customer = new Models.Customer();
                    customer.IdentityUserId = applicationUser.Id;
                    _context.Customers.Add(customer);
                }
                customer.FirstName = customerData.FirstName;
                customer.LastName = customerData.LastName;
                customer.PhoneNumber = customerData.Phone;
                _context.SaveChanges();

                Models.Address address = null;
                if (customer.AddressId != null)
                {
                    address = _context.Address.Single(a => a.Id == customer.AddressId);
                }
                else
                {
                    address = new Address();
                    _context.Address.Add(address);
                }

                address.Street = customerData.Address1;
                address.Street2 = customerData.Address2;
                address.PostalCode = customerData.PostalCode;
                address.City = customerData.City;
                address.Country = customerData.Country;
                address.PostalCode = customerData.PostalCode;
                address.State = customerData.State;
                _context.SaveChanges();

                if (customer.AddressId == null)
                {
                    customer.AddressId = address.Id;
                    _context.SaveChanges();
                }
            }
        }

        void ImportCategories(Data data)
        {
            foreach (var categoryData in data.Categories)
            {
                var category = _context.Categories.SingleOrDefault(c => c.LinkName == categoryData.LinkName);
                if(category == null)
                {
                    category = new Category();
                    _context.Categories.Add(category);
                }
                category.Name = categoryData.Name;
                category.LinkName = categoryData.LinkName;
                if(categoryData.Parent != "")
                {
                    category.ParentCategoryId = _context.Categories.Single(c => c.LinkName == categoryData.Parent).Id;
                }
                else
                {
                    category.ParentCategoryId = null;
                }
                _context.SaveChanges();
            }
        }

        void ImportSpecifications(Data data)
        {
            Dictionary<string, Models.SpecificationType> stringToType = new Dictionary<string, SpecificationType>()
            {
                { "Boolean", Models.SpecificationType.Boolean }, {"Interger", Models.SpecificationType.Interger }, {"String", Models.SpecificationType.String}, {"Decimal", Models.SpecificationType.Decimal}
            };

            foreach (var specificationData in data.Specifications)
            {
                var specification = _context.Specifications.SingleOrDefault(c => c.Name == specificationData.Name);
                if (specification == null)
                {
                    specification = new Models.Specification();
                    _context.Specifications.Add(specification);
                }
                specification.Name = specificationData.Name;
                specification.LongName = specificationData.LongName;
                specification.Unity = specificationData.Unity;
                specification.Type = stringToType[specificationData.Type];
                _context.SaveChanges();
            }
        }

        void ImportCompareGroups(Data data)
        {
            foreach(var compareGroupData in data.CompareGroups)
            {
                var compareGroup = _context.CompareGroups.SingleOrDefault(c => c.LinkName == compareGroupData.LinkName);
                if(compareGroup == null)
                {
                    compareGroup = new Models.CompareGroup();
                    _context.CompareGroups.Add(compareGroup);
                }
                compareGroup.LinkName = compareGroupData.LinkName;
                compareGroup.Name = compareGroupData.Name;
                _context.SaveChanges();
            }
        }

        void ImportCompareGroupSpecifications(Data data)
        {
            foreach (var compareGroupSpecificationData in data.CompareGroupSpecifications)
            {
                var compareGroup = _context.CompareGroups.Single(c => c.LinkName == compareGroupSpecificationData.CompareGroup);
                var specification = _context.Specifications.Single(s => s.Name == compareGroupSpecificationData.Specification);

                var compareGroupSpecification = _context.CompareGroupSpecifications.SingleOrDefault(c => c.CompareGroupId == compareGroup.Id && c.SpecificationId == specification.Id);
                if (compareGroupSpecification == null)
                {
                    compareGroupSpecification = new Models.CompareGroupSpecification();
                    compareGroupSpecification.CompareGroupId = compareGroup.Id;
                    compareGroupSpecification.SpecificationId = specification.Id;
                    _context.CompareGroupSpecifications.Add(compareGroupSpecification);
                    _context.SaveChanges();
                }               
            }
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