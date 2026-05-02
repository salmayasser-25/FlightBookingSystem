using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



[Area(SD.Admin_Area)]
[Route("Admin/[controller]/[action]")]
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

    // GET: /Admin/Airport/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Admin/Airport/Create
    [HttpPost]
    public IActionResult Create(AirportVM vm)
    {
        var airport = new Airport
        {
            Name = vm.Name,
            City = vm.City,
            Country = vm.Country,
            IataCode = vm.IataCode?.ToUpper(),
            Latitude = vm.Latitude,
            Longitude = vm.Longitude
        };

        _context.Airports.Add(airport);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

}