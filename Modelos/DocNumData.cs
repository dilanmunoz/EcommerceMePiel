using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class DocNumData
    {
        [Required]
        public string CardCode { get; set; }
        [Required]
        public string CardName { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        public string UUID { get; set; }
    }
}
