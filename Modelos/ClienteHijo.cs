using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class ClienteHijo
    {
        [Required]
        [JsonProperty("CodigoCliente")]
        public string CodigoCliente { get; set; }

        [Required]
        [JsonProperty("NombreCliente")]
        public string NombreCliente { get; set; }

        [Required]
        [JsonProperty("RFC")]
        public string RFC { get; set; }

        [Required]
        [JsonProperty("Almacen")]
        public string Almacen { get; set; }
        [Required]
        [JsonProperty("Credito")]
        public bool Credito { get; set; }

        [Required]
        [JsonProperty("DireccionFactura")]
        public Direccion DireccionFactura { get; set; }

        [Required]
        [JsonProperty("DireccionesEntrega")]
        public List<Direccion> DireccionesEntrega { get; set; }
    }
}
