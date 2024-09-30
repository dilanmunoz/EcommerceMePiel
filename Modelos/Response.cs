using Newtonsoft.Json;

namespace EcommerceMePiel.Modelos
{
    public class Response<T>
    {
        [JsonProperty("Exito")]
        public bool Exito { get; set; }
        [JsonProperty("Respuesta")]
        public T Respuesta { get; set; }
    }
}
