using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using DopaMarket.ViewModels;

namespace DopaMarket.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        ApplicationDbContext _context;

        public ClientController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId().ToString();
            var client = _context.Clients.SingleOrDefault(c => c.IdentityUserId == userId);
            var address = _context.Address.SingleOrDefault(c => c.Id == client.AddressId);

            var clientFormViewModel = new ClientFormViewModel();
            clientFormViewModel.Client = client;
            clientFormViewModel.Address = address;

            return View(clientFormViewModel);
        }

        public ActionResult Edit()
        {
            var userId = User.Identity.GetUserId().ToString();
            var client = _context.Clients.SingleOrDefault(c => c.IdentityUserId == userId);

            var clientFormViewModel = new ClientFormViewModel();
            clientFormViewModel.Client  = client;
            if (client.AddressId != null)
                clientFormViewModel.Address = _context.Address.Single(a => a.Id == client.AddressId);
            else
                clientFormViewModel.Address = new Address();

            return View("ClientForm", clientFormViewModel);
        }

        [HttpPost]
        public ActionResult Save(ClientFormViewModel clientFormViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("ClientForm", clientFormViewModel);
            }

            Address addressInDB;
            if (clientFormViewModel.Address.Id != 0)
                addressInDB = _context.Address.Single<Address>(a => a.Id == clientFormViewModel.Address.Id);
            else
                addressInDB = new Address();

            addressInDB.Street     = clientFormViewModel.Address.Street;
            addressInDB.Street2    = clientFormViewModel.Address.Street2;
            addressInDB.City       = clientFormViewModel.Address.City;
            addressInDB.State      = clientFormViewModel.Address.State;
            addressInDB.PostalCode = clientFormViewModel.Address.PostalCode;
            addressInDB.Country    = clientFormViewModel.Address.Country;

            _context.Address.Add(addressInDB);
            _context.SaveChanges();

            var clientInDB = _context.Clients.Single<Client>(c => c.Id == clientFormViewModel.Client.Id);
            clientInDB.Name = clientFormViewModel.Client.Name;
            clientInDB.PhoneNumber = clientFormViewModel.Client.PhoneNumber;
            clientInDB.Birthday = clientFormViewModel.Client.Birthday;
            clientInDB.AddressId = addressInDB.Id;

            _context.SaveChanges();

            return RedirectToAction("Index", "Client");
        }
    }
}