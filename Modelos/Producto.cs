using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class Producto
    {
        //[Required] 
        //public int Id { get; set; }
        [Required]
        [JsonProperty("CodigoSAP")]
        public string CodigoSAP { get; set; }
        [Required]
        [JsonProperty("SKU")]
        public string SKU { get; set; }
        [Required]
        [JsonProperty("Descripcion")]
        public string Descripcion { get; set; }
        [Required]
        [JsonProperty("Categorias")]
        public List<string> Categorias { get; set; }
        [Required]
        [JsonProperty("Marca")]
        public string Marca { get; set; }
        [Required]
        [JsonProperty("Laboratorio")]
        public string Laboratorio { get; set; }
        [Required]
        [JsonProperty("Precio")]
        public double Precio { get; set; }
        [Required]
        [JsonProperty("PrecioPublico")]
        public double PrecioPublico { get; set; }
        [Required]
        [JsonProperty("GrupoClientes")]
        public List<GrupoCliente> GrupoClientes { get; set; }
        [Required]
        [JsonProperty("Activo")]
        public string Activo { get; set; }
        [Required]
        [JsonProperty("Estatus")]
        public string Estatus { get; set; }
    }
}
