using CarServiceManagement.Data;
using CarServiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceManagement.Controllers
{
    public class ServiceOrdersController : Controller
    {
        private readonly ApplicationDbContext _kontekst;

        public ServiceOrdersController(ApplicationDbContext kontekst)
        {
            _kontekst = kontekst;
        }
        public IActionResult Index()
        {
            var radniNalozi = _kontekst.ServiceOrders
                .Include(nalog => nalog.Customer)
                .Include(nalog => nalog.Vehicle)
                .ToList();

            return View(radniNalozi);
        }
        public IActionResult Create()
        {
            var musterije = _kontekst.Customers
                .Select(musterija => new
                {
                    musterija.Id,
                    ImeIPrezime = musterija.FirstName + " " + musterija.LastName
                })
                .ToList();

            ViewBag.Musterije = new SelectList(
                musterije, "Id", "ImeIPrezime");

            var vozila = _kontekst.Vehicles
                .Select(vozilo => new
                {
                    vozilo.Id,
                    NazivVozila = vozilo.Brand + " " + vozilo.Model
                })
                .ToList();

            ViewBag.Vozila = new SelectList(
                vozila, "Id", "NazivVozila");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceOrder nalog)
        {
            // Proveravamo da li izabrano vozilo pripada izabranoj mušteriji
            var vozilo = _kontekst.Vehicles
                .FirstOrDefault(v =>
                    v.Id == nalog.VehicleId &&
                    v.CustomerId == nalog.CustomerId);

            if (vozilo == null)
            {
                ModelState.AddModelError(
                    "VehicleId",
                    "Izabrano vozilo ne pripada ovoj mušteriji.");
            }

            if (ModelState.IsValid)
            {
                _kontekst.ServiceOrders.Add(nalog);
                _kontekst.SaveChanges();

                return RedirectToAction("Index");
            }

            // Ponovo popunjavamo padajuće menije ako postoje greške
            var musterije = _kontekst.Customers
                .Select(musterija => new
                {
                    musterija.Id,
                    ImeIPrezime = musterija.FirstName + " " + musterija.LastName
                })
                .ToList();

            ViewBag.Musterije = new SelectList(
                musterije, "Id", "ImeIPrezime", nalog.CustomerId);

            var vozila = _kontekst.Vehicles
                .Select(v => new
                {
                    v.Id,
                    NazivVozila = v.Brand + " " + v.Model
                })
                .ToList();

            ViewBag.Vozila = new SelectList(
                vozila, "Id", "NazivVozila", nalog.VehicleId);

            return View(nalog);
        }

    }
}