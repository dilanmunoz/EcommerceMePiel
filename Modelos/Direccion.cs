using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class Direccion
    {
        [Required]
        [JsonProperty("CodigoDireccion")]
        public string CodigoDireccion { get; set; }

        [Required]
        [JsonProperty("Calle")]
        public string Calle { get; set; }

        [Required]
        [JsonProperty("Colonia")]
        public string Colonia { get; set; }

        [Required]
        [JsonProperty("CP")]
        public string CP { get; set; }

        [Required]
        [JsonProperty("Ciudad")]
        public string Ciudad { get; set; }

        [Required]
        [JsonProperty("Pais")]
        public string Pais { get; set; }

        [Required]
        [JsonProperty("Estado")]
        public string Estado { get; set; }
    }
}
