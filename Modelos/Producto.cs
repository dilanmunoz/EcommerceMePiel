using System.ComponentModel.DataAnnotations;

namespace EcommerceMePiel.Modelos
{
    public class Producto
    {
        [Required] 
        public int Id { get; set; }
        [Required]
        public string CodigoSAP { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string ImagenUrlProducto { get; set; }
        [Required]
        public List<string> Categorias { get; set; }
        [Required]
        public string Marca { get; set; }
        [Required]
        public string Laboratorio { get; set; }
        [Required]
        public double Precio { get; set; }
        [Required]
        public double PrecioPublico { get; set; }
        [Required]
        public List<GrupoCliente> GrupoClientes { get; set; }
        [Required]
        public string Activo { get; set; }
        [Required]
        public string Estatus { get; set; }
    }
}
