namespace EcommerceMePiel.Modelos
{
    public class Response<T>
    {
        public bool Exito { get; set; }
        public T Respuesta { get; set; }
    }
}
