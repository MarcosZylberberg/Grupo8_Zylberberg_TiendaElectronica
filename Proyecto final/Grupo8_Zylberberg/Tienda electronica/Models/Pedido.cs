using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_electronica.Models
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPedido { get; set; }

        [Required]
        [ForeignKey(nameof(Cliente))]
        public int IdCliente { get; set; }

        public Cliente Cliente { get; set; }

        [Required]
        public bool completado { get; set; } = false;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [EnumDataType(typeof(MetodosDePago))]
        public MetodosDePago metodo { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();

        [Precision(18, 2)]
        public decimal Total => Detalles.Sum(d => d.PrecioUnitario * d.Cantidad);
    }
}
