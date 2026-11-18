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
            var poslednjiRadniNalozi = _kontekst.ServiceOrders
                .Include(nalog => nalog.Customer)
                .Include(nalog => nalog.Vehicle)
                .OrderByDescending(nalog => nalog.DateCreated)
                .Take(5)
                .ToList();

            ViewBag.PoslednjiRadniNalozi = poslednjiRadniNalozi;
            var brojNalogaPoStatusu = _kontekst.ServiceOrders
                .GroupBy(nalog => nalog.Status)
                .Select(grupa => new
                {
                    Status = grupa.Key,
                    Broj = grupa.Count()
                })
                .ToList();

            ViewBag.BrojNalogaPoStatusu = brojNalogaPoStatusu;
            ViewBag.BrojVozilaPoMusteriji = brojVozilaPoMusteriji;
            ViewBag.UkupanPrihod = _kontekst.ServiceOrders.Where(nalog => nalog.Status == "Završen").Sum(nalog => (decimal?)nalog.Price) ?? 0;
            int trenutnaGodina = DateTime.Now.Year;

            var prihodiPoMesecima = _kontekst.ServiceOrders
                .Where(nalog =>
                    nalog.Status == "Završen" &&
                    nalog.DateCompleted.HasValue &&
                    nalog.DateCompleted.Value.Year == trenutnaGodina)
                .GroupBy(nalog => nalog.DateCompleted!.Value.Month)
                .Select(grupa => new
                {
                    Mesec = grupa.Key,
                    Prihod = grupa.Sum(nalog => nalog.Price)
                })
                .ToDictionary(x => x.Mesec, x => x.Prihod);

            ViewBag.PrihodiPoMesecima = Enumerable.Range(1, 12)
                .Select(mesec => new
                {
                    Mesec = new DateTime(trenutnaGodina, mesec, 1)
                        .ToString("MMM"),
                    Prihod = prihodiPoMesecima.GetValueOrDefault(mesec, 0m)
                })
                .ToList();
            ViewBag.PoslednjaVozila = poslednjaVozila;
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
