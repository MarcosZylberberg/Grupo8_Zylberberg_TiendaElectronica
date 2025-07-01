using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tienda_electronica.Context;
using Tienda_electronica.Models;

namespace Tienda_electronica.Controllers
{
    public class PedidosController : Controller
    {
        private TiendaElectronicaDatabaseContext _context;

        public PedidosController(TiendaElectronicaDatabaseContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> AddToCart(int productoId)
        {
            // 1) Intentamos leer el claim NameIdentifier de forma segura
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out userId))
            {
                // Fallback: obtenemos el username y buscamos el usuario en BD
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized();

                var userEntity = await _context.Usuarios
                    .SingleOrDefaultAsync(u => u.username == username);
                if (userEntity == null)
                    return Unauthorized();

                userId = userEntity.IdUsuario;
            }

            // 2) Carga (o crea) el pedido “abierto”
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.IdCliente == userId && !p.completado);

            if (pedido == null)
            {
                pedido = new Pedido
                {
                    IdCliente = userId,
                    Fecha = DateTime.UtcNow,
                    completado = false,
                    metodo = MetodosDePago.Efectivo
                };
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
            }
            // lo añadimos al contexto YA con su detalle
            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return NotFound();
            await AgregarOIncrementarDetalleAsync(
                pedido.IdPedido,
                productoId,
                cantidad: 1,
                producto.Precio);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Pedidos");
        }

        private async Task AgregarOIncrementarDetalleAsync(
            int pedidoId,
            int productoId,
            int cantidad,
            decimal precio)
        {
            // 1) Busca si ya hay un detalle para este pedido y producto
            var detalle = await _context.Detalles
                .FirstOrDefaultAsync(d =>
                    d.IdPedido == pedidoId &&
                    d.IdProducto == productoId);

            if (detalle != null)
            {
                // 2a) Si existe, súmale la cantidad
                detalle.Cantidad += cantidad;
            }
            else
            {
                // 2b) Si no existe, créalo 
                detalle = new DetallePedido
                {
                    IdPedido = pedidoId,
                    IdProducto = productoId,
                    Cantidad = cantidad,
                    PrecioUnitario = precio
                };
                _context.Detalles.Add(detalle);

                // Guardar el detalle en la lista Detalles del pedido
                var pedido = await _context.Pedidos
                    .FirstOrDefaultAsync(p => p.IdPedido == pedidoId);

                if (pedido != null)
                {
                    pedido.Detalles.Add(detalle);
                }
            }
            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalize(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
                return NotFound();
            if (pedido.completado)
                return BadRequest("El pedido ya está completado.");

            // 1) Validar stock
            var sinStock = pedido.Detalles
                .Where(d => d.Cantidad > d.Producto.CantidadStock)
                .Select(d => d.Producto.Nombre)
                .ToList();

            if (sinStock.Any())
            {
                TempData["ErrorStock"] = "No hay stock para: " + string.Join(", ", sinStock);
                return RedirectToAction(nameof(Index));
            }

            // 2) Descontar stock y calcular Subtotal
            foreach (var det in pedido.Detalles)
            {
                det.Producto.CantidadStock -= det.Cantidad;
                det.Subtotal = det.Cantidad * det.PrecioUnitario;
            }

            // 3) Marca pedido como completado
            pedido.completado = true;

            // 4) Calcula y asigna el Total 
            pedido.Total = pedido.Detalles.Sum(d => d.Subtotal);

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            TempData["Success"] = "Compra finalizada con éxito.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Cliente,Usuario")]
        public async Task<IActionResult> Index()
        {
            // 1) Intento leer el claim NameIdentifier de forma segura
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId;
      
            // Obtengo el username y busco el usuario en BD
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var userEntity = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.username == username);
            if (userEntity == null)
                return Unauthorized();

            userId = userEntity.IdUsuario;
            
            // Siempre cargamos detalles + producto
            var query = _context.Pedidos
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (userEntity is Cliente)
            {
                // Si es cliente, sólo sus pedidos
                query = query.Where(p => p.IdCliente == userId);
            }
            // Si es Usuario (admin), dejamos query sin filtrar para que traiga todos

            // Ordenamos: abiertos primero, luego completados más recientes
            var pedidos = await query
                .OrderBy(p => p.completado)
                .ThenByDescending(p => p.Fecha)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null) return NotFound();

            return View(pedido);
        }

        // GET: Pedidos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPedido,IdCliente,completado,Fecha,metodo")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPedido,IdCliente,completado,Fecha,metodo")] Pedido pedido)
        {
            if (id != pedido.IdPedido)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.IdPedido))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.IdPedido == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.IdPedido == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int detalleId, int cantidad)
        {
            // 1) Buscar el detalle
            var detalle = await _context.Detalles.FindAsync(detalleId);
            if (detalle == null)
                return NotFound();

            // 2) Actualizar la cantidad
            detalle.Cantidad = cantidad;
            await _context.SaveChangesAsync();

            // 3) Calcular el nuevo subtotal y devolverlo en JSON
            var nuevoSubtotal = detalle.Cantidad * detalle.PrecioUnitario;
            return Json(new
            {
                success = true,
                nuevoSubtotal = nuevoSubtotal
            });
        }
    }
}
