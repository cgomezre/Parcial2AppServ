using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Parcial2.Models
{
    public class Prenda_Models
    {
        [Key]
        public int IdPrenda { get; set; }

        public string TipoPrenda { get; set; }
        public string Descripcion { get; set; }
        public float Valor { get; set; }

        [ForeignKey("Cliente")]
        public string Cliente { get; set; }
        public Cliente ClienteInfo { get; set; }

        public ICollection<FotoPrenda> Fotos { get; set; }
    }
}