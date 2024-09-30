using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class Conflicto
    {
        [Required]
        [JsonProperty("Codigo")]
        public int Codigo { get; set; }
        [Required]
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }
    }
}
