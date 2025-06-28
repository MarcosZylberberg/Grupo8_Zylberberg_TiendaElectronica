using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Tienda_electronica.Context;
using Tienda_electronica.Models;

namespace Tienda_electronica.Controllers
{
    public class AccountController : Controller
    {
        private readonly TiendaElectronicaDatabaseContext _context;

        public AccountController(TiendaElectronicaDatabaseContext context)
            => _context = context;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            // 1) Recupera el usuario (podría ser Cliente o Usuario)
            var user = await _context.Usuarios
                .SingleOrDefaultAsync(u => u.username == Username && u.password == Password);

            // 2) Calcula referer (fallback a Home/Index)
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
                referer = Url.Action("Index", "Home");

            // 3) Si no existe → error y reabre modal
            if (user == null)
            {
                TempData["LoginError"] = "Usuario o contraseña incorrectos";
                return Redirect(referer);
            }

            // 4) Crea la cookie
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim("IdUsuario", user.IdUsuario.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Cerrar sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Determinar URL de retorno
            string referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                referer = Url.Action("Index", "Home");
            }

            return Redirect(referer);
        }
    }
}