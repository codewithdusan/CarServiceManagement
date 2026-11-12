using CarServiceManagement.Data;
using CarServiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarServiceManagement.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _kontekst;

        public VehiclesController(ApplicationDbContext kontekst)
        {
            _kontekst = kontekst;
        }

        public IActionResult Index()
        {
            var vozila = _kontekst.Vehicles
                .Include(vozilo => vozilo.Customer)
                .ToList();

            return View(vozila);
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

            ViewBag.Musterije = new SelectList(musterije, "Id", "ImeIPrezime");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vehicle vozilo)
        {
            if (ModelState.IsValid)
            {
                _kontekst.Vehicles.Add(vozilo);
                _kontekst.SaveChanges();

                return RedirectToAction("Index");
            }

            var musterije = _kontekst.Customers
                .Select(musterija => new
                {
                    musterija.Id,
                    ImeIPrezime = musterija.FirstName + " " + musterija.LastName
                })
                .ToList();

            ViewBag.Musterije = new SelectList(
                musterije, "Id", "ImeIPrezime", vozilo.CustomerId);

            return View(vozilo);
        }
        public IActionResult Edit(int id)
        {
            var vozilo = _kontekst.Vehicles.Find(id);

            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Vehicle vozilo)
        {
            if (ModelState.IsValid)
            {
                var postojeceVozilo = _kontekst.Vehicles.Find(vozilo.Id);

                if (postojeceVozilo == null)
                {
                    return NotFound();
                }

                postojeceVozilo.Brand = vozilo.Brand;
                postojeceVozilo.Model = vozilo.Model;
                postojeceVozilo.Year = vozilo.Year;
                postojeceVozilo.RegistrationNumber = vozilo.RegistrationNumber;

                _kontekst.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(vozilo);
        }
    }
}