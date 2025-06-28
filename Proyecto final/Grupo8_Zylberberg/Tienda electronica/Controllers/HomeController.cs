using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tienda_electronica.Models;
using Tienda_electronica.Context;
using System.Linq;

namespace Tienda_electronica.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TiendaElectronicaDatabaseContext _context;

        public HomeController(ILogger<HomeController> logger, TiendaElectronicaDatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Traemos los destacados de forma síncrona
            var destacados = _context.Productos
                .Where(p => p.EsDestacado)
                .ToList();

            // Lo pasamos en el model
            return View(destacados);
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
