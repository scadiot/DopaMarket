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
            ImportBrands(parsedData);
            ImportItems(parsedData);
            ImportOrders(parsedData);
            UpdateCategoriesItemsCount();

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

        void ImportBrands(Data data)
        {
            foreach (var brandData in data.Brands)
            {
                var brand = _context.Brands.SingleOrDefault(b => b.LinkName == brandData.LinkName);
                if (brand == null)
                {
                    brand = new Models.Brand();
                    _context.Brands.Add(brand);
                }
                brand.LinkName = brandData.LinkName;
                brand.Name = brandData.Name;
                _context.SaveChanges();
            }
        }

        void ImportItems(Data data)
        {
            foreach (var itemData in data.Items)
            {
                var item = _context.Items.SingleOrDefault(c => c.LinkName == itemData.LinkName);
                if (item == null)
                {
                    item = new Models.Item();
                    _context.Items.Add(item);
                }

                item.Name = itemData.Name;

                item.LinkName = itemData.LinkName;
                item.InsertDate = DateTime.Parse(itemData.InsertDate);
                item.TinyDescriptive = itemData.TinyDescriptive;
                item.Descriptive = itemData.Descriptive;
                item.CurrentPrice = itemData.CurrentPrice;
                item.Brand = _context.Brands.Single(b => b.LinkName == itemData.Brand);
                item.MainCategoryId = _context.Categories.Single(c => c.LinkName == itemData.MainCategory).Id;
                item.Weight = itemData.Weight;
                item.Width = itemData.Width;
                item.Height = itemData.Height;
                item.Length = itemData.Length;
                item.SKU = "";
                _context.SaveChanges();

                ImportReview(item, itemData.Reviews);
                ImportItemCategories(item, itemData.Categories);
                ImportItemKeywords(item, itemData.Keywords);
                ImportFeatures(item, itemData.Features);
                ImportItemSpecifications(item, itemData.ItemSpecifications);
                ComputeAverageRate(item);
            }
        }

        void ImportReview(Models.Item item, IEnumerable<Review> reviews)
        {
            foreach (var reviewData in reviews)
            {
                var applicationUser = _context.Users.Single(u => u.Email == reviewData.Email);
                var customer = _context.Customers.Single(c => c.IdentityUserId == applicationUser.Id);

                var review = _context.ItemReviews.SingleOrDefault(r => r.ItemId == item.Id && r.CustomerId == customer.Id);
                if(review == null)
                {
                    review = new ItemReview();
                    review.ItemId = item.Id;
                    review.CustomerId = customer.Id;
                    _context.ItemReviews.Add(review);
                }
                review.Title = reviewData.Title;
                review.Text = reviewData.Text;
                review.Note = reviewData.Rate;
                review.Date = DateTime.Now;
                _context.SaveChanges();
            }
        }

        void ImportItemCategories(Models.Item item, IEnumerable<String> categories)
        {
            foreach (var categoryLinkName in categories)
            {
                var category = _context.Categories.Single(c => c.LinkName == categoryLinkName);
                var itemCategory = _context.ItemCategories.SingleOrDefault(ic => ic.ItemId == item.Id && ic.CategoryId == category.Id);
                if(itemCategory == null)
                {
                    itemCategory = new ItemCategory();
                    itemCategory.ItemId = item.Id;
                    itemCategory.CategoryId = category.Id;
                    _context.ItemCategories.Add(itemCategory);
                }
                _context.SaveChanges();
            }
        }

        void ImportItemKeywords(Models.Item item, IEnumerable<String> keywords)
        {
            foreach (var word in keywords)
            {
                var keyword = _context.Keywords.SingleOrDefault(k => k.Word == word);
                if(keyword == null)
                {
                    keyword = new Keyword();
                    keyword.Word = word;
                    _context.Keywords.Add(keyword);
                    _context.SaveChanges();
                }

                var ItemKeyword = _context.ItemKeywords.SingleOrDefault(ic => ic.ItemId == item.Id && ic.KeywordId == keyword.Id);
                if (ItemKeyword == null)
                {
                    ItemKeyword = new ItemKeyword();
                    ItemKeyword.ItemId = item.Id;
                    ItemKeyword.KeywordId = keyword.Id;
                    _context.ItemKeywords.Add(ItemKeyword);
                }
                _context.SaveChanges();
            }
        }

        void ImportFeatures(Models.Item item, IEnumerable<String> features)
        {
            var exisitingFeatures = _context.ItemFeatures.Where(f => f.ItemId == item.Id);
            _context.ItemFeatures.RemoveRange(exisitingFeatures);
            _context.SaveChanges();

            foreach (var featureString in features)
            {
                var newFeature = new Models.ItemFeature();
                newFeature.ItemId = item.Id;
                newFeature.Text = featureString;
                _context.ItemFeatures.Add(newFeature);
            }
            _context.SaveChanges();
        }

        void ImportItemSpecifications(Models.Item item, IEnumerable<ItemSpecification> ItemSpecifications)
        {
            foreach(var itemSpecificationData in ItemSpecifications)
            {
                var specification = _context.Specifications.Single(s => s.Name == itemSpecificationData.specification);
                var itemSpecification = _context.ItemSpecifications.SingleOrDefault(i => i.ItemId == item.Id && i.SpecificationId == specification.Id);
                if(itemSpecification == null)
                {
                    itemSpecification = new Models.ItemSpecification();
                    itemSpecification.ItemId = item.Id;
                    itemSpecification.SpecificationId = specification.Id;
                    _context.ItemSpecifications.Add(itemSpecification);
                }
                itemSpecification.DecimalValue = 0;
                itemSpecification.StringValue = "";
                itemSpecification.BooleanValue = false;
                itemSpecification.IntegerValue = 0;
                switch (specification.Type)
                {
                    case SpecificationType.Boolean:
                        itemSpecification.BooleanValue = itemSpecificationData.BooleanValue;
                        break;
                    case SpecificationType.Interger:
                        itemSpecification.IntegerValue = itemSpecificationData.IntegerValue;
                        break;
                    case SpecificationType.String:
                        itemSpecification.StringValue = itemSpecificationData.StringValue;
                        break;
                    case SpecificationType.Decimal:
                        itemSpecification.DecimalValue = itemSpecificationData.DecimalValue;
                        break;
                }
                _context.SaveChanges();
            }
        }

        void ImportOrders(Data parsedData)
        {
            Dictionary<string, OrderStatus> StringToOrderStatus = new Dictionary<string, OrderStatus>()
            {
                { "InProWaitProcessgress", OrderStatus.WaitProcess }, { "Processing", OrderStatus.Processing }, { "WaitQualityCheck", OrderStatus.WaitQualityCheck }, { "QualityChecking", OrderStatus.QualityChecking },
                { "WaitDispatching", OrderStatus.WaitDispatching },{ "Dispatched", OrderStatus.Dispatched },{ "Canceled", OrderStatus.Canceled },{ "Delayed", OrderStatus.Delayed },{ "Delivered", OrderStatus.Delivered }
            };

            foreach (var orderData in parsedData.Orders)
            {
                var applicationUser = _context.Users.Single(u => u.Email == orderData.Customer);
                var customer = _context.Customers.Single(c => c.IdentityUserId == applicationUser.Id);

                var order = _context.Orders.SingleOrDefault(o => o.Key == orderData.Key);
                if (order == null)
                {
                    order = new Models.Order();
                    order.Key = orderData.Key;
                    _context.Orders.Add(order);
                }
                order.CustomerId = customer.Id;
                order.ExpeditionPrice = orderData.ExpeditionPrice;
                order.TotalPrice = orderData.TotalPrice;
                order.Status = StringToOrderStatus[orderData.Status];
                order.Date = DateTime.Parse(orderData.DateTime);
                _context.SaveChanges();

                ImportOrderItems(order, orderData.Items);
            }
        }

        void ImportOrderItems(Models.Order order, IEnumerable<OrderItem> orderItems)
        {
            foreach(var orderItemData in orderItems)
            {
                var item = _context.Items.Single(i => i.LinkName == orderItemData.Item);
                var orderItem = _context.OrderItems.SingleOrDefault(oi => oi.ItemId == item.Id);
                if(orderItem == null)
                {
                    orderItem = new Models.OrderItem();
                    orderItem.ItemId = item.Id;
                    orderItem.OrderId = order.Id;
                }
                orderItem.Count = orderItemData.Quantity;
                orderItem.Price = orderItemData.Price;
                _context.SaveChanges();
            }
        }

        void ImportOrderNotifications(Models.Order order, IEnumerable<OrderNotification> orderNotifications)
        {
            Dictionary<string, OrderNotificationType> StringToOrderNotificationType = new Dictionary<string, OrderNotificationType>()
            {
                { "Processed", OrderNotificationType.Processed }, { "QualityCheckPassed", OrderNotificationType.QualityCheckPassed }, { "Dispatched", OrderNotificationType.Dispatched},
                { "Delivered", OrderNotificationType.Delivered },{ "Canceled", OrderNotificationType.Canceled },{ "Return", OrderNotificationType.Return }
            };

            var existingNotifications = _context.OrderNotifications.Where(n => n.OrderId == order.Id);
            _context.OrderNotifications.RemoveRange(existingNotifications);
            _context.SaveChanges();

            foreach (var orderNotificationData in orderNotifications)
            {
                var orderNotification = new Models.OrderNotification();
                orderNotification.DateTime = DateTime.Parse(orderNotificationData.DateTime);
                orderNotification.Text = orderNotificationData.Text;
                orderNotification.Type = StringToOrderNotificationType[orderNotificationData.Type];
                _context.SaveChanges();
            }
        }

        void ComputeAverageRate(Models.Item item)
        {
            var rates = _context.ItemReviews.Where(r => r.ItemId == item.Id).Select(r => r.Note).ToArray();
            item.AverageRating = (decimal)rates.Sum() / rates.Count();
            _context.SaveChanges();
        }

        void UpdateCategoriesItemsCount()
        {
            var itemCategories = _context.ItemCategories.ToArray();
            foreach (var categories in _context.Categories.ToArray())
            {
                categories.ItemsCount = itemCategories.Where(ic => ic.CategoryId == categories.Id).Count();
            }
            _context.SaveChanges();
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
            public string Title { get; set; }
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

        public class Brand
        {
            public string Name { get; set; }
            public string LinkName { get; set; }
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

        public class Order
        {
            public string Key { get; set; }
            public string Customer { get; set; }
            public string DateTime { get; set; }
            public string Status { get; set; }
            public decimal ExpeditionPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public IList<OrderItem> Items { get; set; }
            public IList<OrderNotification> Notifications { get; set; }
        }

        public class OrderItem
        {
            public string Item { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        public class OrderNotification
        {
            public string Type { get; set; }
            public string Text { get; set; }
            public string DateTime { get; set; }
        }

        public class Data
        {
            public IList<Customer> Customers { get; set; }
            public IList<Categorie> Categories { get; set; }
            public IList<Specification> Specifications { get; set; }
            public IList<CompareGroup> CompareGroups { get; set; }
            public IList<CompareGroupSpecification> CompareGroupSpecifications { get; set; }
            public IList<Brand> Brands { get; set; }
            public IList<Item> Items { get; set; }
            public IList<Order> Orders { get; set; }
        }
    }
}