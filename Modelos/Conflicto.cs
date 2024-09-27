using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class Conflicto
    {
        [Required]
        public int Codigo { get; set; }
        [Required]
        public string Descripcion { get; set; }
    }
}
