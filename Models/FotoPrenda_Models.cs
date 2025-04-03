using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Parcial2.Models
{
    public class FotoPrenda_Models
    {

        [Key]

        public int idFoto { get; set; }

        public string FotoPrendaNombre { get; set; }

        [ForeignKey("Prenda")]

        public int idPrenda { get; set; }

        public Prenda Prenda { get; set; }

    }

}