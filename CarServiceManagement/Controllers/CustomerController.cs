using CarServiceManagement.Data;
using CarServiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarServiceManagement.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _kontekst;

        public CustomersController(ApplicationDbContext kontekst)
        {
            _kontekst = kontekst;
        }

        public IActionResult Index(string pretraga, string sortiranje)
        {
            var musterije = _kontekst.Customers
                .Include(musterija => musterija.Vehicles)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pretraga))
            {
                musterije = musterije.Where(musterija =>
                    musterija.FirstName.Contains(pretraga) ||
                    musterija.LastName.Contains(pretraga) ||
                    musterija.Phone.Contains(pretraga));
            }
            if (sortiranje == "ime_asc")
            {
                musterije = musterije.OrderBy(m => m.FirstName);
            }
            else if (sortiranje == "ime_desc")
            {
                musterije = musterije.OrderByDescending(m => m.FirstName);
            }
            else if (sortiranje == "prezime_asc")
            {
                musterije = musterije.OrderBy(m => m.LastName);
            }
            else if (sortiranje == "prezime_desc")
            {
                musterije = musterije.OrderByDescending(m => m.LastName);
            }
            else if (sortiranje == "najvise_vozila")
            {
                musterije = musterije.OrderByDescending(m => m.Vehicles.Count);
            }
            else if (sortiranje == "najmanje_vozila")
            {
                musterije = musterije.OrderBy(m => m.Vehicles.Count);
            }

            ViewBag.UkupnoMusterija = musterije.Count();
            ViewBag.UkupnoVozila = _kontekst.Vehicles.Count();
            return View(musterije.ToList());
        }
        // Prikazuje formular za dodavanje mušterije
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            var musterija = _kontekst.Customers
                .Include(m => m.Vehicles)
                .FirstOrDefault(m => m.Id == id);

            if (musterija == null)
            {
                return NotFound();
            }

            return View(musterija);
        }
        // Prima podatke iz formulara
        [HttpPost]
        public IActionResult Create(Customer musterija)
        {
            if (ModelState.IsValid)
            {
                _kontekst.Customers.Add(musterija);

                _kontekst.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(musterija);
        }
        public IActionResult Edit(int id)
        {
            var musterija = _kontekst.Customers.Find(id);

            if (musterija == null)
            {
                return NotFound();
            }

            return View(musterija);
        }


        [HttpPost]
        public IActionResult Edit(Customer musterija)
        {
            if (ModelState.IsValid)
            {
                _kontekst.Customers.Update(musterija);
                _kontekst.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(musterija);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var musterija = _kontekst.Customers
                .Include(m => m.Vehicles)
                .FirstOrDefault(m => m.Id == id);

            if (musterija == null)
            {
                return NotFound();
            }

            if (musterija.Vehicles.Any())
            {
                TempData["Greska"] = "Nije moguće obrisati mušteriju jer ima registrovana vozila.";
                return RedirectToAction("Index");
            }

            _kontekst.Customers.Remove(musterija);
            _kontekst.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}