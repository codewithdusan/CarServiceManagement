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

        public IActionResult Index(string pretraga, string sortiranje)
        {
            var vozila = _kontekst.Vehicles
                .Include(vozilo => vozilo.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pretraga))
            {
                vozila = vozila.Where(vozilo =>
                    vozilo.RegistrationNumber.Contains(pretraga) ||
                    vozilo.Brand.Contains(pretraga) ||
                    vozilo.Model.Contains(pretraga));
            }

            if (sortiranje == "najstarija")
            {
                vozila = vozila.OrderBy(vozilo => vozilo.Year);
            }
            else if (sortiranje == "najnovija")
            {
                vozila = vozila.OrderByDescending(vozilo => vozilo.Year);
            }

            return View(vozila.ToList());
        }

        public IActionResult Details(int id)
        {
            var vozilo = _kontekst.Vehicles
                .Include(vozilo => vozilo.Customer)
                .FirstOrDefault(vozilo => vozilo.Id == id);

            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        public IActionResult ByCustomer(int id)
        {
            var vozila = _kontekst.Vehicles
                .Include(vozilo => vozilo.Customer)
                .Where(vozilo => vozilo.CustomerId == id)
                .ToList();

            return View("Index", vozila);
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
                postojeceVozilo.CustomerId = vozilo.CustomerId;

                _kontekst.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(vozilo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var vozilo = _kontekst.Vehicles
                .Include(v => v.ServiceOrders)
                .FirstOrDefault(v => v.Id == id);

            if (vozilo == null)
            {
                return NotFound();
            }

            if (vozilo.ServiceOrders.Any())
            {
                TempData["Greska"] = "Nije moguće obrisati vozilo jer ima povezane servisne naloge.";
                return RedirectToAction("Index");
            }

            _kontekst.Vehicles.Remove(vozilo);
            _kontekst.SaveChanges();

            TempData["Uspeh"] = "Vozilo je uspešno obrisano.";

            return RedirectToAction("Index");
        }



    }
}