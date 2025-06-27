using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_electronica.Models
{
    public class DetallePedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDetalle { get; set; }

        [Required]
        public int IdPedido { get; set; }
        public Pedido Pedido { get; set; }

        [Required]
        public int IdProducto { get; set; }
        public Producto Producto { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required, Range(0.01, double.MaxValue)]

        [Precision(18, 2)]
        public decimal PrecioUnitario { get; set; }
    }
}
