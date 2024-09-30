using EcommerceMePiel.Modelos;
using Sap.Data.Hana;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Diagnostics.Metrics;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace EcommerceMePiel.Datos
{
    public class Data
    {


        #region Variables

        //Variable Path
        private static string _path = Directory.GetCurrentDirectory();

        //Variables de en Encryptado
        public static string AES256_USER_Key = "hUf3eof71VCa7IIjFlNewZ73yBWjckdX";

        #endregion


        #region Metodo Globales

        //Metodo para obtener parametrizaciones
        public static string Get_Parameterizations(string parameter)
        {
            string yourData = string.Empty;

            string file = _path + @"\Configuracion\Ajustes.txt";
            try
            {
                // Lee todo el contenido del archivo JSON
                string jsonContent = File.ReadAllText(file);

                // Deserializa el JSON a un objeto C# usando Newtonsoft.Json
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonContent);

                switch (parameter)
                {
                    case "ServerSQL":
                        yourData = jsonObject.ServerSQL;
                        break;
                    case "User":
                        yourData = jsonObject.User;
                        break;
                    case "Password":
                        yourData = jsonObject.Password;
                        break;
                    case "MailRemitente":
                        yourData = jsonObject.MailRemitente;
                        break;
                    case "MailDestinatarioError":
                        yourData = jsonObject.MailDestinatarioError;
                        break;
                    case "DbSql":
                        yourData = jsonObject.DbSql;
                        break;
                    case "HanaConec":
                        yourData = jsonObject.HanaConec;
                        break;
                    case "DbSap":
                        yourData = jsonObject.DbSap;
                        break;
                    case "Name":
                        yourData = jsonObject.Name;
                        break;
                    case "HanaServidor":
                        yourData = jsonObject.HanaServidor;
                        break;
                    case "SapDBpass":
                        yourData = jsonObject.SapDBpass;
                        break;
                    case "DBNameTest":
                        yourData = jsonObject.DBNameTest;
                        break;
                    case "UserSAP":
                        yourData = jsonObject.UserSAP;
                        break;
                    case "PswSap":
                        yourData = jsonObject.PswSap;
                        break;
                    case "Ip":
                        yourData = jsonObject.Ip;
                        break;
                    case "Token":
                        yourData = jsonObject.Token;
                        break;
                }

            }
            catch (Exception ex)
            {
                string error = "No fue posible obtener el parametro " + parameter + "." + ex.Message.ToString();

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);

                //Avisar de error por correo
                SendEmail(error);

            }

            return yourData;
        }

        //Metodo para guardar Log
        public static void Log(string path, string Message)
        {
            Task.Run(() =>
            {
                using (var sw = new StreamWriter(path, true))
                {
                    //Log Afectacion
                    DateTime date = DateTime.Now;
                    sw.WriteAsync("\n");
                    sw.WriteLineAsync(date + " " + Message);
                }
            });

        }

        //Metodo para enviar correo
        public static void SendEmail(string error)
        {

            //Variable de fecha
            DateTime dateTime = DateTime.Now;
            string fechaFormateada = dateTime.ToString("yyyyMMdd");

            try
            {
                MailMessage msg = new MailMessage();
                string MailRemitente = Get_Parameterizations("MailRemitente");
                MailAddress fromMail = new MailAddress(MailRemitente);
                msg.From = fromMail;

                msg.Subject = "Error en Quotes back";


                string MailDestinatarioError = Get_Parameterizations("MailDestinatarioError");
                msg.To.Add(new MailAddress(MailDestinatarioError));

                msg.Body = @"<h1 style='font-size:20px;'>" + error + @"</h1> 
                         <br /> 
                         <strong></strong> 
                         <br /> 
                         <br /> 
                         <strong></strong> 
                         <br /> 
                         ";


                msg.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient())
                {

                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(MailRemitente, "giIb3OTlMq;+");
                    client.Host = "svgp364.serverneubox.com.mx";
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 100000;
                    client.Send(msg);

                }
            }
            catch (Exception ex)
            {
                string msj = "Error: Al enviar correo, " + ex.Message.ToString();
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", msj);
            }
        }

        //Generar token
        public static string getToken(string user, string password)
        {
            string token = user + "#" + "ApiEcommerce" + "#" + password + "#" + "2024";
            token = AES256_Encriptar(AES256_USER_Key, token);
            return token;
        }

        // Método para encriptar texto
        public static string AES256_Encriptar(string key, string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(texto);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        // Método para decencriptar texto
        public static string AES256_Desencriptar(string key, string textoCifrado)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(textoCifrado);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static Token validateTokenInRequest(string token_On_Request)
        {
            Token tokenRequest = new Token();
            try
            {

                token_On_Request = CorregirToken(token_On_Request);
                string tokenDescodificado = AES256_Desencriptar(AES256_USER_Key, token_On_Request);
                tokenRequest.Area = tokenDescodificado.Split('#')[0];
                tokenRequest.Nombre = tokenDescodificado.Split('#')[1];
                tokenRequest.Empresa = tokenDescodificado.Split('#')[2];
                tokenRequest.Año = tokenDescodificado.Split('#')[3];

                return tokenRequest;

            }
            catch (Exception)
            {
                return tokenRequest;
            }
        }

        public static string CorregirToken(string token)
        {
            return token.Replace("%2F", "/");
        }

        #endregion


        #region Metodos en servicio obtenerProductos

        //Metodo para obtener productos de sap
        public static List<Producto> obtenerProductos()
        {
            List<Producto> Lista = new List<Producto>();
            List<string> categorias = new List<string>();

            string Universal = string.Empty;

            string DB = string.Empty;
            //int counter = 0;

            string StrSql = string.Empty;

            try
            {
                //Conexion a SAP
                //Con = new HanaConnection(Get_Parameterizations("HanaConec"));
                using (var Con = new HanaConnection(Get_Parameterizations("HanaConec")))
                {
                    Con.Open();

                    //DB = productive == "YES" ? Get_Parameterizations("DbSap") : Get_Parameterizations("DBNameTest");
                    DB = Get_Parameterizations("DbSap");

                    
                            StrSql = $@"SELECT DISTINCT 
                                        T0.""ItemCode"" ""CodigoSAP"",
                                        T0.""CodeBars"" ""SKU"",
                                        ifnull(T0.""ItemName"",'Sin datos') ""Descripcion"",
                                        T0.""U_Marca"" ""Marca"",
                                        T2.""CardName"" ""Laboratorio"",
                                        (SELECT IFNULL(""Price"",0) AS ""PPublico"" FROM {DB}.ITM1 WHERE ""ItemCode"" = T0.""ItemCode"" AND ""PriceList"" ='1') AS ""Precio"",
                                        (SELECT IFNULL(""Price"",0) AS ""PPublico"" FROM {DB}.ITM1 WHERE ""ItemCode"" = T0.""ItemCode"" AND ""PriceList"" ='3') AS ""Precio Publico"",
                                        case
                                        when T0.""validFor"" = 'Y' then 'Activo'
                                        else 'Inactivo'
                                        end as ""Activo"",
                                        ifnull(T0.""U_Estatus"",'Descontinuado'),

                                    CASE WHEN T0.""QryGroup1"" = 'Y' THEN 'Solares' ELSE '0' END AS ""Solares"", 
                                    CASE WHEN T0.""QryGroup2"" = 'Y' THEN 'Piel grasa' ELSE '0' END AS ""Piel grasa"", 
                                    CASE WHEN T0.""QryGroup3"" = 'Y' THEN 'Atópica' ELSE '0' END AS ""Atópica"", 
                                    CASE WHEN T0.""QryGroup4"" = 'Y' THEN 'Cicatrizante' ELSE '0' END AS ""Cicatrizante"", 
                                    CASE WHEN T0.""QryGroup5"" = 'Y' THEN 'Hidratante' ELSE '0' END AS ""Hidratante"", 
                                    CASE WHEN T0.""QryGroup6"" = 'Y' THEN 'Pediatrico' ELSE '0' END AS ""Pediatrico"", 
                                    CASE WHEN T0.""QryGroup7"" = 'Y' THEN 'Sensible' ELSE '0' END AS ""Sensible"", 
                                    CASE WHEN T0.""QryGroup8"" = 'Y' THEN 'Seca' ELSE '0' END AS ""Seca"", 
                                    CASE WHEN T0.""QryGroup9"" = 'Y' THEN 'Antiedad' ELSE '0' END AS ""Antiedad"", 
                                    CASE WHEN T0.""QryGroup10"" = 'Y' THEN 'Capilares' ELSE '0' END AS ""Capilares"", 
                                    CASE WHEN T0.""QryGroup11"" = 'Y' THEN 'Despigmentante' ELSE '0' END AS ""Despigmentante"", 
                                    CASE WHEN T0.""QryGroup12"" = 'Y' THEN 'Suplemento Alimenticio' ELSE '0' END AS ""Suplemento Alimenticio"", 
                                    CASE WHEN T0.""QryGroup13"" = 'Y' THEN 'Intima' ELSE'0' END AS ""Intima"", 
                                    CASE WHEN T0.""QryGroup14"" = 'Y' THEN 'Limpieza' ELSE '0' END AS ""Limpieza"", 
                                    CASE WHEN T0.""QryGroup15"" = 'Y' THEN 'Medico' ELSE '0' END AS ""Medico"", 
                                    CASE WHEN T0.""QryGroup16"" = 'Y' THEN 'Hombre' ELSE '0' END AS ""Hombre"", 
                                    CASE WHEN T0.""QryGroup17"" = 'Y' THEN 'Maquillaje' ELSE '0' END AS ""Maquillaje"", 
                                    CASE WHEN T0.""QryGroup18"" = 'Y' THEN 'Corporales' ELSE '0' END AS ""Corporales"", 
                                    CASE WHEN T0.""QryGroup19"" = 'Y' THEN 'Desodorante' ELSE '0' END AS ""Desodorante"", 
                                    CASE WHEN T0.""QryGroup20"" = 'Y' THEN 'Dermocosmetico' ELSE '0' END AS ""Dermocosmetico"", 
                                    CASE WHEN T0.""QryGroup21"" = 'Y' THEN 'Medicamento' ELSE '0' END AS ""Medicamento"", 
                                    CASE WHEN T0.""QryGroup22"" = 'Y' THEN 'Oftalmología' ELSE '0' END AS ""Oftalmología"",
                                    CASE WHEN T0.""QryGroup23"" = 'Y' THEN 'Dispositivo' ELSE '0' END AS ""Dispositivo""

                                        FROM 
                                        {DB}.OITM T0  
                                        left JOIN  {DB}.ITM1 T1 ON T0.""ItemCode"" = T1.""ItemCode""
                                        INNER JOIN {DB}.OCRD T2 ON T0.""CardCode"" = T2.""CardCode""
                                                    
                                        WHERE  
                                        T1.""PriceList"" = '1'
                                        --and OIT.""validFor"" = 'Y'
                                        AND T0.""CodeBars"" is not null 
                                        and T0.""InvntItem"" = 'Y'

                                        group by T0.""ItemCode"", T0.""CodeBars"", T0.""ItemName"",T0.""U_Marca"",T2.""CardName"", T0.""validFor"",T0.""U_Estatus"",T0.""QryGroup1"",T0.""QryGroup2"",T0.""QryGroup3"",T0.""QryGroup4"",T0.""QryGroup5"",T0.""QryGroup6"",T0.""QryGroup6"",T0.""QryGroup7"",T0.""QryGroup8"",T0.""QryGroup9"",T0.""QryGroup10"",T0.""QryGroup11"",T0.""QryGroup12"",T0.""QryGroup13"",T0.""QryGroup14"",T0.""QryGroup15"",T0.""QryGroup16"",T0.""QryGroup17"",T0.""QryGroup18"",T0.""QryGroup19"",T0.""QryGroup20"",T0.""QryGroup21"",T0.""QryGroup22"",T0.""QryGroup23""

                                        ORDER BY T0.""ItemCode"" asc";
                            

                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            //counter++;
                            var producto = new Producto
                            {
                                //Id = counter,
                                CodigoSAP = reader.IsDBNull(0) ? "No data" : reader.GetString(0),
                                SKU = reader.IsDBNull(1) ? "No data" : reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? "No data" : reader.GetString(2),
                                Marca = reader.IsDBNull(3) ? "No data" : reader.GetString(3),
                                Laboratorio = reader.IsDBNull(4) ? "No data" : reader.GetString(4),
                                Precio = reader.IsDBNull(5) ? 0 : reader.GetDouble(5),
                                PrecioPublico = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
                                Activo = reader.IsDBNull(7) ? "No data" : reader.GetString(7),
                                Estatus = reader.IsDBNull(8) ? "No data" : reader.GetString(8),
                                GrupoClientes = null,
                                ImagenUrlProducto = null,
                                //Categorias = obtenerCategoriasProducto(reader.IsDBNull(0) ? "No data" : reader.GetString(0), DB),
                            };
                            for (int i = 9; i <= 31; i++)
                            {
                                Universal = reader.GetString(i);
                                if (Universal != "0")
                                {
                                    //categorias = categorias + Universal + ",";
                                    categorias.Add(Universal);
                                }
                            }
                            //if (categorias != null)
                            //{
                            //    categorias = categorias.TrimEnd(',');
                            //}
                            producto.Categorias = categorias;
                            categorias = new List<string>();
                            Lista.Add(producto);
                        }

                        // Procesar listClients según sea necesario
                    }
                }
            }
            catch (Exception ex)
            {
                string error = "Error al obtener listado de productos " + ex.Message.ToString();

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);

                return new List<Producto>();

            }

            return Lista;
        }

        //Metodo para obtener categorias de producto
        //public static List<Categoria> obtenerCategoriasProducto(string itemCode, string Base)
        //{
        //    List<Categoria> lista = new List<Categoria>();
        //    string Universal;

        //    try
        //    {
        //        using (var Conn = new HanaConnection(Get_Parameterizations("HanaConec")))
        //        {
        //            Conn.Open();


        //            string StrSql = $@"SELECT 

        //                            CASE WHEN T0.""QryGroup1"" = 'Y' THEN 'Solares' ELSE '0' END AS ""Solares"", 
        //                            CASE WHEN T0.""QryGroup2"" = 'Y' THEN 'Piel grasa' ELSE '0' END AS ""Piel grasa"", 
        //                            CASE WHEN T0.""QryGroup3"" = 'Y' THEN 'Atópica' ELSE '0' END AS ""Atópica"", 
        //                            CASE WHEN T0.""QryGroup4"" = 'Y' THEN 'Cicatrizante' ELSE '0' END AS ""Cicatrizante"", 
        //                            CASE WHEN T0.""QryGroup5"" = 'Y' THEN 'Hidratante' ELSE '0' END AS ""Hidratante"", 
        //                            CASE WHEN T0.""QryGroup6"" = 'Y' THEN 'Pediatrico' ELSE '0' END AS ""Pediatrico"", 
        //                            CASE WHEN T0.""QryGroup7"" = 'Y' THEN 'Sensible' ELSE '0' END AS ""Sensible"", 
        //                            CASE WHEN T0.""QryGroup8"" = 'Y' THEN 'Seca' ELSE '0' END AS ""Seca"", 
        //                            CASE WHEN T0.""QryGroup9"" = 'Y' THEN 'Antiedad' ELSE '0' END AS ""Antiedad"", 
        //                            CASE WHEN T0.""QryGroup10"" = 'Y' THEN 'Capilares' ELSE '0' END AS ""Capilares"", 
        //                            CASE WHEN T0.""QryGroup11"" = 'Y' THEN 'Despigmentante' ELSE '0' END AS ""Despigmentante"", 
        //                            CASE WHEN T0.""QryGroup12"" = 'Y' THEN 'Suplemento Alimenticio' ELSE '0' END AS ""Suplemento Alimenticio"", 
        //                            CASE WHEN T0.""QryGroup13"" = 'Y' THEN 'Intima' ELSE'0' END AS ""Intima"", 
        //                            CASE WHEN T0.""QryGroup14"" = 'Y' THEN 'Limpieza' ELSE '0' END AS ""Limpieza"", 
        //                            CASE WHEN T0.""QryGroup15"" = 'Y' THEN 'Medico' ELSE '0' END AS ""Medico"", 
        //                            CASE WHEN T0.""QryGroup16"" = 'Y' THEN 'Hombre' ELSE '0' END AS ""Hombre"", 
        //                            CASE WHEN T0.""QryGroup17"" = 'Y' THEN 'Maquillaje' ELSE '0' END AS ""Maquillaje"", 
        //                            CASE WHEN T0.""QryGroup18"" = 'Y' THEN 'Corporales' ELSE '0' END AS ""Corporales"", 
        //                            CASE WHEN T0.""QryGroup19"" = 'Y' THEN 'Desodorante' ELSE '0' END AS ""Desodorante"", 
        //                            CASE WHEN T0.""QryGroup20"" = 'Y' THEN 'Dermocosmetico' ELSE '0' END AS ""Dermocosmetico"", 
        //                            CASE WHEN T0.""QryGroup21"" = 'Y' THEN 'Medicamento' ELSE '0' END AS ""Medicamento"", 
        //                            CASE WHEN T0.""QryGroup22"" = 'Y' THEN 'Oftalmología' ELSE '0' END AS ""Oftalmología"",
        //                            CASE WHEN T0.""QryGroup23"" = 'Y' THEN 'Dispositivo' ELSE '0' END AS ""Dispositivo""

        //                            FROM 
        //                            {Base}.OITM T0

        //                            WHERE 
        //                                T0.""ItemCode"" = '{itemCode}'";

        //            using (var cmd = new HanaCommand(StrSql, Conn))
        //            using (var reader = cmd.ExecuteReader())
        //            {

        //                while (reader.Read())
        //                {

        //                    //1
        //                    Universal = reader.GetString(0);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //2
        //                    Universal = reader.GetString(1);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //3
        //                    Universal = reader.GetString(2);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //4
        //                    Universal = reader.GetString(3);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //5
        //                    Universal = reader.GetString(4);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //6
        //                    Universal = reader.GetString(5);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //7
        //                    Universal = reader.GetString(6);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //8
        //                    Universal = reader.GetString(7);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //9
        //                    Universal = reader.GetString(8);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //10
        //                    Universal = reader.GetString(9);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //11
        //                    Universal = reader.GetString(10);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //12
        //                    Universal = reader.GetString(11);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //13
        //                    Universal = reader.GetString(12);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //14
        //                    Universal = reader.GetString(13);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //15
        //                    Universal = reader.GetString(14);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //16
        //                    Universal = reader.GetString(15);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //17
        //                    Universal = reader.GetString(16);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //18
        //                    Universal = reader.GetString(17);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //19
        //                    Universal = reader.GetString(18);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //20
        //                    Universal = reader.GetString(19);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //21
        //                    Universal = reader.GetString(20);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //22
        //                    Universal = reader.GetString(21);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                    //23
        //                    Universal = reader.GetString(22);
        //                    if (Universal != "0")
        //                    {
        //                        var categoria = new Categoria
        //                        {
        //                            categoria = Universal,
        //                        };
        //                        lista.Add(categoria);
        //                    }

        //                }

        //                // Procesar listClients según sea necesario
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string error = "Error al obtener propertys de articulo " + ex.Message.ToString();

        //        DateTime date = DateTime.Now;
        //        string fechaFormateada = date.ToString("yyyyMMdd");

        //        //Generar log
        //        Log(_path + @"Log\" + fechaFormateada + ".txt", error);

        //        return new List<Categoria>();
        //    }

        //    return lista;
        //}

        //Metodo para aumentar tamaño


        #endregion


    }
}
