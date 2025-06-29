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

            // 1) Cargar cliente
            var cliente = await _context.Usuarios
                .OfType<Cliente>()
                .SingleOrDefaultAsync(u => u.IdUsuario == userId);
            if (cliente == null)
                return Unauthorized();

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
                // 2b) Si no existe, créalo de nuevo
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
        }


        // Pseudocódigo:
        // 1. Obtener el usuario actual (User).
        // 2. Si el usuario tiene el rol "Cliente":
        //    - Obtener su IdUsuario (por claim o username).
        //    - Filtrar los pedidos por IdCliente == IdUsuario.
        // 3. Si el usuario tiene el rol "Usuario" (o cualquier otro):
        //    - Devolver todos los pedidos.
        // 4. Retornar la vista con la lista correspondiente.

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Cliente"))
            {
                // Intentar obtener el IdUsuario del claim
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int userId;
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out userId))
                {
                    // Fallback: buscar por username
                    var username = User.Identity?.Name;
                    if (string.IsNullOrEmpty(username))
                        return Unauthorized();

                    var userEntity = await _context.Usuarios
                        .SingleOrDefaultAsync(u => u.username == username);
                    if (userEntity == null)
                        return Unauthorized();

                    userId = userEntity.IdUsuario;
                }

                var pedidosCliente = await _context.Pedidos
                    .Where(p => p.IdCliente == userId)
                    .Include(p => p.Detalles)               // traigo la colección Detalles
                        .ThenInclude(d => d.Producto)
                    .ToListAsync();
                return View(pedidosCliente);
            }

            // Si el usuario no tiene el rol "Cliente", devolver todos los pedidos
            var todosLosPedidos = await _context.Pedidos.ToListAsync();
            return View(todosLosPedidos);
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
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
    }
}
