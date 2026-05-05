using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



[Area(SD.Admin_Area)]

public class AirportController : Controller
{
    private readonly ApplicationDbContext _context;

    public AirportController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/Airport/Index
    [HttpGet]
    public IActionResult Index()
    {
        var airports = _context.Airports.ToList();
        return View(airports);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Airport airport)
    {
        airport.IataCode = airport.IataCode?.ToUpper();

        _context.Airports.Add(airport);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
    [HttpGet]
    
    public IActionResult Edit(int id)
    {
        var airport = _context.Airports.Find(id);

        if (airport == null)
            return NotFound();

        return View(airport);
    }

    [HttpPost]
    public IActionResult Edit(Airport airport)
    {
        airport.IataCode = airport.IataCode?.ToUpper();

        _context.Airports.Update(airport);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }
   
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var airport = _context.Airports.Find(id);

        if (airport == null)
            return NotFound();

        bool hasFlights = _context.Flights
            .Any(f => f.DepartureAirportId == id || f.ArrivalAirportId == id);

        if (hasFlights)
        {
            TempData["Error"] = "Cannot delete airport linked to flights!";
            return RedirectToAction("Index");
        }

        _context.Airports.Remove(airport);
        _context.SaveChanges();

        TempData["Success"] = "Airport deleted successfully";

        return RedirectToAction("Index");
    }

}