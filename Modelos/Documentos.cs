using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class Documentos
    {
        [Required]
        [JsonProperty("PDF_Base64")]
        public string PDF_Base64 { get; set; }

        [Required]
        [JsonProperty("XML_BASE64")]
        public string XML_BASE64 { get; set; }

    }
}
