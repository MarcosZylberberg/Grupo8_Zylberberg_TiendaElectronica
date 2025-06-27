using System.ComponentModel.DataAnnotations;

namespace Tienda_electronica.Models
{
    public class Cliente : Usuario
    {
        [Required, EmailAddress]
        public string Mail { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required, Phone]
        public string Telefono { get; set; }
    }
}
