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
        public IActionResult Index(string pretraga, string status, string sortiranje)
        {
            var upit = _kontekst.ServiceOrders
                .Include(nalog => nalog.Customer)
                .Include(nalog => nalog.Vehicle)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pretraga))
            {
                upit = upit.Where(nalog =>
                    (nalog.Customer != null &&
                     (nalog.Customer.FirstName.Contains(pretraga) ||
                      nalog.Customer.LastName.Contains(pretraga))) ||

                    (nalog.Vehicle != null &&
                     (nalog.Vehicle.Brand.Contains(pretraga) ||
                      nalog.Vehicle.Model.Contains(pretraga) ||
                      nalog.Vehicle.RegistrationNumber.Contains(pretraga)))
                );
            }
            if (!string.IsNullOrEmpty(status))
            {
                upit = upit.Where(nalog => nalog.Status == status);
            }

            if (sortiranje == "najstariji")
            {
                upit = upit.OrderBy(nalog => nalog.DateCreated);
            }
            else
            {
                upit = upit.OrderByDescending(nalog => nalog.DateCreated);
            }

            var radniNalozi = upit.ToList();
            ViewBag.UkupnoNaloga = _kontekst.ServiceOrders.Count();

            ViewBag.NaCekanju = _kontekst.ServiceOrders
                .Count(n => n.Status == "Na čekanju");

            ViewBag.Utoku = _kontekst.ServiceOrders
                .Count(n => n.Status == "U toku");

            ViewBag.Zavrseno = _kontekst.ServiceOrders
                .Count(n => n.Status == "Završen");

            ViewBag.Pretraga = pretraga;
            ViewBag.Status = status;
            ViewBag.Sortiranje = sortiranje;

            return View(radniNalozi);
        }

        public IActionResult Details(int id)
        {
            var nalog = _kontekst.ServiceOrders
                .Include(n => n.Customer)
                .Include(n => n.Vehicle)
                .FirstOrDefault(n => n.Id == id);

            if (nalog == null)
            {
                return NotFound();
            }

            return View(nalog);
        }

        public IActionResult Edit(int id)
        {
            var nalog = _kontekst.ServiceOrders
                .FirstOrDefault(n => n.Id == id);

            if (nalog == null)
            {
                return NotFound();
            }

            return View(nalog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ServiceOrder izmenjenNalog)
        {
            if (ModelState.IsValid)
            {
                var postojeciNalog = _kontekst.ServiceOrders
                    .FirstOrDefault(n => n.Id == izmenjenNalog.Id);

                if (postojeciNalog == null)
                {
                    return NotFound();
                }

                postojeciNalog.Description = izmenjenNalog.Description;
                postojeciNalog.Status = izmenjenNalog.Status;
                postojeciNalog.Price = izmenjenNalog.Price;

                if (izmenjenNalog.Status == "Završen")
                {
                    postojeciNalog.DateCompleted =
                        postojeciNalog.DateCompleted ?? DateTime.Now;
                }
                else
                {
                    postojeciNalog.DateCompleted = null;
                }


                _kontekst.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(izmenjenNalog);
        }


        public IActionResult Delete(int id)
        {
            var nalog = _kontekst.ServiceOrders
                .Include(n => n.Customer)
                .Include(n => n.Vehicle)
                .FirstOrDefault(n => n.Id == id);

            if (nalog == null)
            {
                return NotFound();
            }

            return View(nalog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var nalog = _kontekst.ServiceOrders
                .FirstOrDefault(n => n.Id == id);

            if (nalog == null)
            {
                return NotFound();
            }

            _kontekst.ServiceOrders.Remove(nalog);

            _kontekst.SaveChanges();

            return RedirectToAction("Index");
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

            ViewBag.Vozila = new SelectList(Enumerable.Empty<SelectListItem>(),"Value","Text");

            return View();
        }

        [HttpGet]
        public IActionResult GetVehiclesByCustomer(int customerId)
        {
            var vozila = _kontekst.Vehicles
                .Where(v => v.CustomerId == customerId)
                .Select(v => new
                {
                    id = v.Id,
                    naziv = v.Brand + " " + v.Model
                })
                .ToList();

            return Json(vozila);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceOrder nalog)
        {
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
            var musterije = _kontekst.Customers
                .Select(musterija => new
                {
                    musterija.Id,
                    ImeIPrezime = musterija.FirstName + " " + musterija.LastName
                })
                .ToList();

            ViewBag.Musterije = new SelectList(
                musterije, "Id", "ImeIPrezime", nalog.CustomerId);

            var vozila = _kontekst.Vehicles.Where(v => v.CustomerId == nalog.CustomerId).Select(v => new{v.Id,NazivVozila = v.Brand + " " + v.Model}).ToList();

            ViewBag.Vozila = new SelectList(
                vozila,
                "Id",
                "NazivVozila",
                nalog.VehicleId
            );

            ViewBag.Vozila = new SelectList(
                vozila, "Id", "NazivVozila", nalog.VehicleId);

            return View(nalog);
        }

    }
}