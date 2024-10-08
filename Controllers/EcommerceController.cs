using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EcommerceMePiel.Modelos;
using EcommerceMePiel.Datos;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Linq;
using EcommerceMePiel.Filtros;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Http.HttpResults;


namespace EcommerceMePiel.Controllers
{
    //[Route("SapMepiel")]
    [ApiController]
    public class EcommerceController : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(
        Summary = "Obtener Token",
        Description = "Este servicio se encarga de generar un token a partir de un usuario y contraseña.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("GetToken")]
        public IActionResult GetToken(string usuario, string contraseña)
        {
            try
            {

                if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(usuario))
                {
                    Response<Conflicto> response = new Response<Conflicto>();
                    response.Exito = false;

                    Conflicto conflicto = new Conflicto();
                    conflicto.Descripcion = "Es necesario incluir un usuario y/o contraseña";
                    conflicto.Codigo = 404;

                    response.Respuesta = conflicto;
                    return NotFound(response);
                }
                else
                {
                    string g = Data.getToken(usuario, contraseña);
                    return Ok(g);
                }


            }
            catch (Exception x)
            {
                Response<Conflicto> response = new Response<Conflicto>();
                response.Exito = false;

                Conflicto conflicto = new Conflicto();
                conflicto.Descripcion = "Error al obtener Token";
                conflicto.Codigo = 500;

                response.Respuesta = conflicto;
                return StatusCode(500, response);
            }

        }


        [HttpGet]
        [SwaggerOperation(
        Summary = "Obtener todos los productos de la base de datos de SAP",
        Description = "Este servicio se encarga de obtener todos los productos disponibles en la base de datos de SAP. Este servicio nesesita un token.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorization]
        //[CustomHeaderRequired] // Este endpoint requiere el encabezado
        [Route("GetProducts")]
        public IActionResult obtenerProductos(/*[FromHeader(Name = "Authorization")] string Authorization*/)
        {
            string Authorization = Request.Headers["Authorization"];

            try
            {
                if (!string.IsNullOrEmpty(Authorization))
                {
                    Authorization = Authorization.Replace("Bearer ", "");

                    string ValidToken = Data.Get_Parameterizations("Token");

                    string ValidTokenTest = Data.Get_Parameterizations("TokenTest");

                    if (ValidToken == Authorization)
                    {

                        List<Producto> a = Data.obtenerProductos("YES");

                        if (a.Count < 1)
                        {
                            Response<Conflicto> response = new Response<Conflicto>();
                            response.Exito = false;
                            response.Respuesta.Descripcion = "Error al obtener listado de productos";
                            response.Respuesta.Codigo = 404;
                            return NotFound(response);
                        }
                        else
                        {
                            Response<List<Producto>> response = new Response<List<Producto>>()
                            {
                                Exito = true,
                                Respuesta = a
                            };

                            // Calcular el tamaño de la respuesta en bytes
                            //string jsonResponse = JsonSerializer.Serialize(response);
                            //long responseSize = System.Text.Encoding.UTF8.GetByteCount(jsonResponse);

                            // Opcional: Puedes registrar el tamaño o devolverlo en el encabezado
                            //Response.Headers.Add("X-Response-Size", responseSize.ToString());

                            return Ok(response);
                        }
                    }
                    else if (ValidTokenTest == Authorization)
                    {
                        List<Producto> a = Data.obtenerProductos("NO");

                        if (a.Count < 1)
                        {
                            Response<Conflicto> response = new Response<Conflicto>();
                            response.Exito = false;
                            response.Respuesta.Descripcion = "Error al obtener listado de productos";
                            response.Respuesta.Codigo = 404;
                            return NotFound(response);
                        }
                        else
                        {
                            Response<List<Producto>> response = new Response<List<Producto>>()
                            {
                                Exito = true,
                                Respuesta = a
                            };

                            // Calcular el tamaño de la respuesta en bytes
                            //string jsonResponse = JsonSerializer.Serialize(response);
                            //long responseSize = System.Text.Encoding.UTF8.GetByteCount(jsonResponse);

                            // Opcional: Puedes registrar el tamaño o devolverlo en el encabezado
                            //Response.Headers.Add("X-Response-Size", responseSize.ToString());

                            return Ok(response);
                        }
                    }
                    else
                    {
                        Response<Conflicto> response = new Response<Conflicto>();
                        Conflicto oConflicto = new Conflicto();
                        oConflicto.Descripcion = "Es necesario incluir un token valido";
                        oConflicto.Codigo = 404;

                        response.Exito = false;
                        response.Respuesta = oConflicto;
                        return Unauthorized(response);
                    }
                }
                else
                {
                    Response<Conflicto> response = new Response<Conflicto>();
                    response.Exito = false;

                    Conflicto conflicto = new Conflicto();
                    conflicto.Descripcion = "Es necesario incluir un token";
                    conflicto.Codigo = 404;

                    response.Respuesta = conflicto;
                    return NotFound(response);
                }
            }
            catch (Exception x)
            {
                Response<Conflicto> response = new Response<Conflicto>();
                response.Exito = false;

                Conflicto conflicto = new Conflicto();
                conflicto.Descripcion = "Error al obtener listado de productos";
                conflicto.Codigo = 500;

                response.Respuesta = conflicto;
                return StatusCode(500, response);
            }

        }


        #region codigo de pruebas comentado
        //[HttpGet("endpoint")]
        //[CustomHeaderRequired] // Este endpoint requiere el encabezado
        //public IActionResult GetData([FromHeader(Name = "Authorization")] string authorization)
        //{
        //    if (string.IsNullOrWhiteSpace(authorization))
        //    {
        //        return BadRequest("El encabezado Authorization es requerido.");
        //    }

        //    // Asegúrate de que el encabezado siga el formato esperado
        //    if (!authorization.StartsWith("Bearer "))
        //    {
        //        return BadRequest("El formato del encabezado Authorization es incorrecto. Debe ser 'Bearer <token>'.");
        //    }

        //    // Procesa el token
        //    var token = authorization.Substring("Bearer ".Length).Trim();

        //    // Aquí puedes validar el token o hacer algo con él
        //    // ...

        //    return Ok("Token recibido: " + token);
        //}
        #endregion


        [HttpGet("GetXmlPdf/{DocNum:int}", Name = "GetDocument")]
        [SwaggerOperation(
        Summary = "Método para obtener documentos.",
        Description = "Este servicio se encarga de obtener el documento PDF y el XML a partir del número de factura. Este servicio nesesita un token.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Route("GetXmlPdf")]
        public IActionResult GetDocument(int DocNum)
        {
            try
            {
                if (DocNum == 0)
                {
                    Response<Conflicto> response = new Response<Conflicto>();
                    response.Exito = false;

                    Conflicto conflicto = new Conflicto();
                    conflicto.Descripcion = "Es necesario incluir el numero de factura.";
                    conflicto.Codigo = 404;

                    response.Respuesta = conflicto;
                    return NotFound(response);
                }
                else
                {
                    string Authorization = Request.Headers["Authorization"];

                    if (!string.IsNullOrEmpty(Authorization))
                    {
                        Authorization = Authorization.Replace("Bearer ", "");

                        string ValidToken = Data.Get_Parameterizations("Token");

                        string ValidTokenTest = Data.Get_Parameterizations("TokenTest");

                        if (ValidToken == Authorization)
                        {

                            // LLamado de funcion para traer DocNumData
                            DocNumData data = Data.GetDocumentData(DocNum, "YES");

                            if (data.Equals(null) || data.UUID == null || data.UUID == "")
                            {
                                Response<Conflicto> response = new Response<Conflicto>();
                                response.Exito = false;

                                Conflicto conflicto = new Conflicto();
                                conflicto.Descripcion = "Documento no encontrado";
                                conflicto.Codigo = 404;

                                response.Respuesta = conflicto;
                                return NotFound(response);
                            }

                            // LLamado de funcion para traer documentos
                            var Documentos = Data.GetDocumentos(data);

                            if (Documentos.Equals(null) || Documentos.PDF_Base64 == null)
                            {
                                Response<Conflicto> response = new Response<Conflicto>();
                                response.Exito = false;

                                Conflicto conflicto = new Conflicto();
                                conflicto.Descripcion = "Documento no encontrado";
                                conflicto.Codigo = 404;

                                response.Respuesta = conflicto;
                                return NotFound(response);
                            }

                            return Ok(Documentos);
                        }
                        else if (ValidTokenTest == Authorization)
                        {
                            // LLamado de funcion para traer DocNumData
                            DocNumData data = Data.GetDocumentData(DocNum, "NO");

                            if (data.Equals(null) || data.UUID == null || data.UUID == "")
                            {
                                Response<Conflicto> response = new Response<Conflicto>();
                                response.Exito = false;

                                Conflicto conflicto = new Conflicto();
                                conflicto.Descripcion = "Documento no encontrado";
                                conflicto.Codigo = 404;

                                response.Respuesta = conflicto;
                                return NotFound(response);
                            }

                            // LLamado de funcion para traer documentos
                            var Documentos = Data.GetDocumentos(data);

                            if (Documentos.Equals(null) || Documentos.PDF_Base64 == null)
                            {
                                Response<Conflicto> response = new Response<Conflicto>();
                                response.Exito = false;

                                Conflicto conflicto = new Conflicto();
                                conflicto.Descripcion = "Documento no encontrado";
                                conflicto.Codigo = 404;

                                response.Respuesta = conflicto;
                                return NotFound(response);
                            }

                            return Ok(Documentos);
                        }
                        else
                        {
                            Response<Conflicto> response = new Response<Conflicto>();
                            response.Exito = false;

                            Conflicto conflicto = new Conflicto();
                            conflicto.Descripcion = "Es necesario incluir un token valido";
                            conflicto.Codigo = 404;

                            response.Respuesta = conflicto;
                            return NotFound(response);
                        }
                    }
                    else
                    {
                        Response<Conflicto> response = new Response<Conflicto>();
                        response.Exito = false;

                        Conflicto conflicto = new Conflicto();
                        conflicto.Descripcion = "Es necesario incluir un token";
                        conflicto.Codigo = 404;

                        response.Respuesta = conflicto;
                        return NotFound(response);
                    }
                }
            }
            catch (Exception ex)
            {
                Response<Conflicto> response = new Response<Conflicto>();
                response.Exito = false;

                Conflicto conflicto = new Conflicto();
                conflicto.Descripcion = "Error al obtener documentos";
                conflicto.Codigo = 500;

                response.Respuesta = conflicto;
                return StatusCode(500, response);
            }
        }


        [HttpGet]
        [SwaggerOperation(
        Summary = "Obtener Clientes SAP.",
        Description = "Servicio encargado de consultar todos los clientes activos en SAP, con la caracteristica de Ecommerce='Si'")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("GetClients")]
        public IActionResult GetClients()
        {
            string Authorization = Request.Headers["Authorization"];
            Response<ClientePadre> oResponseClientes = new Response<ClientePadre>();
            Response<Conflicto> oResponseConflicto = new Response<Conflicto>();

            try
            {
                if (string.IsNullOrEmpty(Authorization))
                {
                    oResponseConflicto.Exito = false;

                    Conflicto oConflicto = new Conflicto();
                    oConflicto.Descripcion = "Es necesario incluir un token valido";
                    oConflicto.Codigo = 404;
                    oResponseConflicto.Respuesta = oConflicto;

                    return NotFound(oResponseConflicto);
                }

                Authorization = Authorization.Replace("Bearer ", "");
                string ValidToken = Data.Get_Parameterizations("Token");
                string ValidTokenTest = Data.Get_Parameterizations("TokenTest");

                // Validacion de Token en productivo
                if (ValidToken == Authorization)
                {
                    List<ClientePadre> lsClientes = Data.ConsultaClientes("YES");
                    if (lsClientes.Count < 1)
                    {
                        Response<Conflicto> response = new Response<Conflicto>();
                        response.Exito = false;
                        response.Respuesta.Descripcion = "Error al obtener listado de productos";
                        response.Respuesta.Codigo = 404;
                        return NotFound(response);
                    }
                    else
                    {
                        Response<List<ClientePadre>> response = new Response<List<ClientePadre>>()
                        {
                            Exito = true,
                            Respuesta = lsClientes
                        };
                        return Ok(response);
                    }
                }else
                {
                    Response<Conflicto> response = new Response<Conflicto>();
                    Conflicto oConflicto = new Conflicto();
                    oConflicto.Descripcion = "Es necesario incluir un token valido";
                    oConflicto.Codigo = 404;

                    response.Exito = false;
                    response.Respuesta = oConflicto;
                    return Unauthorized(response); 
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        
        [HttpGet]
        [SwaggerOperation(
        Summary = "Obtener Stock SAP.",
        Description = "Servicio encargado de consultar el Stock completo por almacen")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("GetStocks")]
        public IActionResult GetStocks()
        {
            string Authorization = Request.Headers["Authorization"];
            Response<ArticulosStock> oResponseStock = new Response<ArticulosStock>();
            Response<Conflicto> oResponseConflicto = new Response<Conflicto>();

            try
            {
                if (string.IsNullOrEmpty(Authorization))
                {
                    oResponseConflicto.Exito = false;

                    Conflicto oConflicto = new Conflicto();
                    oConflicto.Descripcion = "Es necesario incluir un token valido";
                    oConflicto.Codigo = 404;
                    oResponseConflicto.Respuesta = oConflicto;

                    return NotFound(oResponseConflicto);
                }

                Authorization = Authorization.Replace("Bearer ", "");
                string ValidToken = Data.Get_Parameterizations("Token");
                string ValidTokenTest = Data.Get_Parameterizations("TokenTest");

                // Validacion de Token en productivo
                if (ValidToken == Authorization)
                {
                    List<ArticulosStock> lsArticuloStock = Data.ConsultaStocks("YES");
                    if (lsArticuloStock.Count < 1)
                    {
                        Response<Conflicto> response = new Response<Conflicto>();
                        response.Exito = false;
                        response.Respuesta.Descripcion = "Error al obtener listado de productos";
                        response.Respuesta.Codigo = 404;
                        return NotFound(response);
                    }
                    else
                    {
                        Response<List<ArticulosStock>> response = new Response<List<ArticulosStock>>()
                        {
                            Exito = true,
                            Respuesta = lsArticuloStock
                        };
                        return Ok(response);
                    }
                }
                else
                {
                    Response<Conflicto> response = new Response<Conflicto>();
                    Conflicto oConflicto = new Conflicto();
                    oConflicto.Descripcion = "Es necesario incluir un token valido";
                    oConflicto.Codigo = 401;

                    response.Exito = false;
                    response.Respuesta = oConflicto;
                    return Unauthorized(response);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}