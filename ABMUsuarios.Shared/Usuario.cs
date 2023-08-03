using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABMUsuarios.Shared
{
    public class Usuario
    {

        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string? Nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string? Email { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }


   
    }
}
