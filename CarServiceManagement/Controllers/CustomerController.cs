using CarServiceManagement.Data;
using CarServiceManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarServiceManagement.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _kontekst;

        public CustomersController(ApplicationDbContext kontekst)
        {
            _kontekst = kontekst;
        }

        public IActionResult Index()
        {
            var musterije = _kontekst.Customers.ToList();

            return View(musterije);
        }
        // Prikazuje formular za dodavanje mušterije
        public IActionResult Create()
        {
            return View();
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
        public IActionResult Delete(int id)
        {
            var musterija = _kontekst.Customers.Find(id);

            if (musterija == null)
            {
                return NotFound();
            }

            _kontekst.Customers.Remove(musterija);
            _kontekst.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}