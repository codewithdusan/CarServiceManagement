using CarServiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CarServiceManagement.Data;
using Microsoft.EntityFrameworkCore;
namespace CarServiceManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _kontekst;

        public HomeController(ApplicationDbContext kontekst)
        {
            _kontekst = kontekst;
        }

        public IActionResult Index()
        {
            ViewBag.UkupnoMusterija = _kontekst.Customers.Count();

            ViewBag.UkupnoVozila = _kontekst.Vehicles.Count();
            var poslednjaVozila = _kontekst.Vehicles
                .Include(vozilo => vozilo.Customer)
                .OrderByDescending(vozilo => vozilo.Id)
                .Take(5)
                .ToList();

            ViewBag.PoslednjaVozila = poslednjaVozila;
            var brojVozilaPoMusteriji = _kontekst.Customers
                .Include(musterija => musterija.Vehicles)
                .Select(musterija => new
                {
                ImeIPrezime = musterija.FirstName + " " + musterija.LastName,
                BrojVozila = musterija.Vehicles.Count
                })
                .ToList();

            ViewBag.BrojVozilaPoMusteriji = brojVozilaPoMusteriji;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
