using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class ClientePadre
    {
        [Required]
        [JsonProperty("Email")]
        public string Email { get; set; }
        [Required]
        [JsonProperty("CodigoAgrupador")]
        public string CodigoAgrupador { get; set; }
        [Required]
        [JsonProperty("TipoCliente")]
        public string TipoCliente { get; set; }
        [Required]
        [JsonProperty("ZonaCliente")]
        public string ZonaCliente { get; set; }
        [Required]
        [JsonProperty("ClientesRelacionados")]
        public List<ClienteHijo> ClientesRelacionados { get; set; }
    }
}
