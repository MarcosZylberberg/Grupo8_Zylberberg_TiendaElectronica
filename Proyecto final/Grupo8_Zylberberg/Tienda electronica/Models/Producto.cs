using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda_electronica.Models
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProducto { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required]
        [Precision(18, 2)]
        public decimal Precio { get; set; }

        [Required]
        public int CantidadStock { get; set; }

        public string? Imagen { get; set; }

        [Display(Name = "Destacado")]
        public bool EsDestacado { get; set; } = false;

        // Esta NO se mapea a BD, solo se usa para bind en el form
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }
    }
}
