using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tienda_electronica.Context;
using Tienda_electronica.Models;

namespace Tienda_electronica.Controllers
{
    public class ProductosController : Controller
    {
        private readonly TiendaElectronicaDatabaseContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductosController(TiendaElectronicaDatabaseContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Productos.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto, IFormFile? ImagenFile)
        {
            if (!ModelState.IsValid)
                return View(producto);

            if (ImagenFile != null && ImagenFile.Length > 0)
            {
                // 1) Carpeta de destino
                var uploadsDir = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(uploadsDir);

                // 2) Saca la extensión original
                var extension = Path.GetExtension(ImagenFile.FileName);

                // 3) Construye un nombre “seguro” a partir del nombre del producto
                //    elimina espacios y caracteres no admitidos
                var safeName = string.Concat(
                    producto.Nombre
                        .ToLowerInvariant()
                        .Where(c => !Path.GetInvalidFileNameChars().Contains(c))
                        .Select(c => c == ' ' ? '_' : c)
                );

                // 4) (Opcional) Evita colisiones añadiendo timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMdd");

                // 5) Monta el nombre final
                var fileName = $"{safeName}_{timestamp}{extension}";

                // 6) Guarda el fichero
                var filePath = Path.Combine(uploadsDir, fileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                await ImagenFile.CopyToAsync(fs);

                // 7) Guarda ese nombre en la BD
                producto.Imagen = fileName;
            }

            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProducto,Nombre,Descripcion,Precio,CantidadStock,Imagen,EsDestacado")] Producto producto)
        {
            if (id != producto.IdProducto)
                return NotFound();

            if (!ModelState.IsValid)
                return View(producto);

            try
            {
                _context.Update(producto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Productos.Any(e => e.IdProducto == producto.IdProducto))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }
}
