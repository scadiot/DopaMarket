using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DopaMarket.Models
{
    // Vous pouvez ajouter des données de profil pour l'utilisateur en ajoutant d'autres propriétés à votre classe ApplicationUser. Pour en savoir plus, consultez https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Notez qu'authenticationType doit correspondre à l'élément défini dans CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Ajouter les revendications personnalisées de l’utilisateur ici
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemCart> ItemCarts { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<ItemKeyword> ItemKeywords { get; set; } 
        public DbSet<ItemFeature> ItemFeatures { get; set; }      
        public DbSet<ItemPrice> ItemPrices { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<ItemSpecification> ItemSpecifications { get; set; }
        public DbSet<CompareGroup> CompareGroups { get; set; }
        public DbSet<CompareGroupSpecification> CompareGroupSpecifications { get; set; }
        public DbSet<ItemReview> ItemReviews { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }
        public DbSet<ItemLink> ItemLinks { get; set; }
        public DbSet<OrderNotification> OrderNotifications { get; set; }
        public DbSet<ItemCompare> ItemCompares { get; set; }
        public DbSet<SpecificationGroup> SpecificationGroups { get; set; }
        
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}