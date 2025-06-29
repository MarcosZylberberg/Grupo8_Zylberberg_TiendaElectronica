using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda_electronica.Models;

using System.Collections.Generic;
namespace Tienda_electronica.Context
{
    public class TiendaElectronicaDatabaseContext : DbContext
    {
        public TiendaElectronicaDatabaseContext(DbContextOptions<TiendaElectronicaDatabaseContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DetallePedido> Detalles { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }

    }
}
