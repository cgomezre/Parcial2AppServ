using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parcial2.Models
{
    public class Cliente_Models
    {
        [Key]
        public string Documento { get; set; }

        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }

        public ICollection<Prenda> Prendas { get; set; }
    }
}