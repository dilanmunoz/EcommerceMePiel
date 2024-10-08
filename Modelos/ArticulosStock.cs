using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class ArticulosStock
    {
        [Required]
        [JsonProperty("CodigoSAP")]
        public string CodigoSAP { get; set; }
        [Required]
        [JsonProperty("SKU")]
        public string SKU { get; set; }
        [Required]
        [JsonProperty("Stock")]
        public List<StockPorAlmacen> Stock { get; set; }
        [Required]
        [JsonProperty("StockFechaCorta")]
        public List<StockPorAlmacen> StockFechaCorta { get; set; }
    }

    public class StockPorAlmacen
    {
        [Required]
        [JsonProperty("CodigoAlmacen")]
        public string CodigoAlmacen { get; set; }
        [Required]
        [JsonProperty("Cantidad")]
        public int Cantidad { get; set; }
    }
}
