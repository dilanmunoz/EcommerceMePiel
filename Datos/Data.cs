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
using System.Security;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

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
                    case "RedPassword":
                        yourData = jsonObject.RedPassword;
                        break;
                    case "RedUser":
                        yourData = jsonObject.RedUser;
                        break;
                    case "TokenTest":
                        yourData = jsonObject.TokenTest;
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


        #region Obtener informacion SAP

        //Metodo para obtener productos de sap
        public static List<Producto> obtenerProductos(string productive)
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

                    DB = productive == "YES" ? Get_Parameterizations("DbSap") : Get_Parameterizations("DBNameTest");
                    //DB = Get_Parameterizations("DbSap");


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

        #region Metodos en servicio GetDocument

        protected static readonly string Path_Pdf = "\\\\172.16.21.249\\b1_shf\\Companies\\SB1CSL\\anexos\\BOVEDA FACTURAS MEPIEL\\";

        protected static readonly string Path_Xml = "\\\\172.16.21.249\\b1_shf\\Companies\\SB1CSL\\XMLTEST\\hanab1\\SB1CSL\\CFDi\\SALC670519A59\\";

        //Consulta DocNumData
        public static DocNumData GetDocumentData(int DocNum, string productive)
        {
            DocNumData Data = new DocNumData();
            string Universal = string.Empty;

            string DB = string.Empty;

            string StrSql = string.Empty;

            try
            {
                //Conexion a SAP
                using (var Con = new HanaConnection(Get_Parameterizations("HanaConec")))
                {
                    Con.Open();

                    DB = productive == "YES" ? Get_Parameterizations("DbSap") : Get_Parameterizations("DBNameTest");
                    //DB = Get_Parameterizations("DbSap");


                    StrSql =
                        $@"
SELECT
    T0.""CardCode"" AS ""Codigo de cliente"",
    T0.""CardName"" AS ""Nombre de cliente"",
    TO_VARCHAR(T0.""DocDate"", 'YYYY-MM') AS ""Fecha de factura"",
    T1.""ReportID""
FROM
    {DB}.OINV T0
LEFT OUTER JOIN
    {DB}.ECM2 T1 ON T0.""DocEntry"" = T1.""SrcObjAbs""
WHERE
    T0.""DocNum"" = '{DocNum}'";


                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            //counter++;
                            Data = new DocNumData()
                            {

                                CardCode = reader.GetString(0),
                                CardName = reader.GetString(1),
                                Date = reader.GetString(2),
                                UUID = reader.IsDBNull(0) ? "" : reader.GetString(3),

                            };

                            if (Data.UUID == "")
                            {
                                return new DocNumData();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = $"Error al obtener datos de factura {DocNum}, {ex.Message.ToString()} ";

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);

                return new DocNumData();

            }

            return Data;
        }

        //Metodo principal traer documentos
        public static Documentos GetDocumentos(DocNumData Data)
        {
            var Full_Path_Pdf = Path_Pdf + $@"{Data.CardName}\I\";
            var Full_Path_Xml = Path_Xml + $@"{Data.Date}\{Data.CardCode}\RF\";

            Documentos documentos = ReadFiles(Full_Path_Pdf, Full_Path_Xml, Data.UUID);

            return documentos;
        }

        //Leer documento en red.
        public static Documentos ReadFiles(string pathpdf, string pathxml, string uuid)
        {
            try
            {
                Documentos documentos = new Documentos();

                var password = Get_Parameterizations("RedPassword");

                var username = Get_Parameterizations("RedUser");

                // Crear password con seguridad
                var securePassword = new SecureString();
                foreach (char c in password)
                {
                    securePassword.AppendChar(c);
                }

                // Mapeo de unidad de red y lectura del archivo
                using (new NetworkConnection(pathpdf, username, securePassword))
                {
                    string pdf = Path.Combine(pathpdf, $"{uuid}.pdf");
                    byte[] fileBytes = File.ReadAllBytes(pdf);
                    documentos.PDF_Base64 = Convert.ToBase64String(fileBytes); /*File.ReadAllText(pdf);*/

                    string XML = Path.Combine(pathxml, $"{uuid}.xml");
                    fileBytes = File.ReadAllBytes(XML);
                    documentos.XML_BASE64 = Convert.ToBase64String(fileBytes);/*File.ReadAllText(XML);*/

                }
                return documentos;
            }
            catch (Exception a)
            {
                string error = $"Error al obtener datos de factura , {a.Message.ToString()} ";

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);

                return new Documentos();
            }

        }


        #endregion

        #region Obtener Clientes SAP
        internal static List<ClientePadre> ConsultaClientes(string sProductive)
        {
            List<ClientePadre> lsClientePadre = new List<ClientePadre>();

            try
            {
                // Obtener los clientes agrupadores
                lsClientePadre = ConsultaClientesPadre(sProductive);

                // Validamos que la lista no este vacia
                if (lsClientePadre.Count > 0)
                {
                    foreach (ClientePadre oClientePadre in lsClientePadre)
                    {
                        List<ClienteHijo> lsClienteHijo = new List<ClienteHijo>();
                        lsClienteHijo = ObtenerClientesHijo(sProductive, oClientePadre.Email);

                        oClientePadre.ClientesRelacionados = lsClienteHijo;
                    }
                }
                return lsClientePadre;
            }
            catch (Exception ex)
            {
                string error = "Error al obtener listado de Clientes " + ex.Message.ToString();

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);

                return new List<ClientePadre>();
            }
        }

        private static List<ClientePadre> ConsultaClientesPadre(string sProductive)
        {
            string Universal = string.Empty;
            string DB = string.Empty;
            string StrSql = string.Empty;
            List<ClientePadre> lsClientePadre = new List<ClientePadre>();

            try
            {
                //Conexion a SAP
                //Con = new HanaConnection(Get_Parameterizations("HanaConec"));
                using (var Con = new HanaConnection(Get_Parameterizations("HanaConec")))
                {
                    Con.Open();
                    DB = sProductive == "YES" ? Get_Parameterizations("DbSap") : Get_Parameterizations("DBNameTest");

                    StrSql = $@"SELECT DISTINCT LOWER(T0.""E_Mail"") ""Email"", T0.""U_CodigoAgrupador"" ""CodigoAgrupador"", T2.""GroupName"" ""GrupoCliente"", 
	                                CASE WHEN T0.""QryGroup4"" = 'Y' THEN TPR4.""GroupName""
		                                 WHEN T0.""QryGroup5"" = 'Y' THEN TPR5.""GroupName""
		                                 WHEN T0.""QryGroup6"" = 'Y' THEN TPR6.""GroupName""
		                                 WHEN T0.""QryGroup7"" = 'Y' THEN TPR7.""GroupName""
		                                 WHEN T0.""QryGroup8"" = 'Y' THEN TPR8.""GroupName"" 
		 	                                ELSE 'N/A' END AS ""ZonaCliente""
                                FROM {DB}.OCRD T0
                                INNER JOIN {DB}.OCPR T1 ON T0.""CardCode"" = T1.""CardCode""
                                INNER JOIN {DB}.OCRG T2 ON T0.""GroupCode"" = T2.""GroupCode""
                                LEFT JOIN {DB}.OCQG TPR4 ON TPR4.""GroupCode"" = 4 AND T0.""QryGroup4"" = 'Y'
                                LEFT JOIN {DB}.OCQG TPR5 ON TPR5.""GroupCode"" = 5 AND T0.""QryGroup5"" = 'Y'
                                LEFT JOIN {DB}.OCQG TPR6 ON TPR6.""GroupCode"" = 6 AND T0.""QryGroup6"" = 'Y'
                                LEFT JOIN {DB}.OCQG TPR7 ON TPR7.""GroupCode"" = 7 AND T0.""QryGroup7"" = 'Y'
                                LEFT JOIN {DB}.OCQG TPR8 ON TPR8.""GroupCode"" = 8 AND T0.""QryGroup8"" = 'Y'
                                WHERE (T0.""U_CodigoAgrupador"" = 'C001' OR T0.""U_CodigoAgrupador"" = 'C002' OR T0.""U_CodigoAgrupador"" = 'C003') 
	                                AND T0.""E_Mail"" != '0'";


                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientePadre oClientePadre = new ClientePadre();

                            //Datos generales
                            if (!string.IsNullOrEmpty(reader["Email"].ToString()) && Regex.IsMatch(reader.GetString(0), "@"))
                                oClientePadre.Email = reader.GetString(0);
                            if (!string.IsNullOrEmpty(reader["CodigoAgrupador"].ToString()))
                                oClientePadre.CodigoAgrupador = reader["CodigoAgrupador"].ToString();
                            if (!string.IsNullOrEmpty(reader["GrupoCliente"].ToString()))
                                oClientePadre.TipoCliente = reader["GrupoCliente"].ToString();
                            if (!string.IsNullOrEmpty(reader["ZonaCliente"].ToString()))
                                oClientePadre.ZonaCliente = reader["ZonaCliente"].ToString();

                            if (!string.IsNullOrEmpty(oClientePadre.Email) && !string.IsNullOrEmpty(oClientePadre.CodigoAgrupador) && !string.IsNullOrEmpty(oClientePadre.TipoCliente) && !string.IsNullOrEmpty(oClientePadre.TipoCliente))
                                lsClientePadre.Add(oClientePadre);
                        }

                        return lsClientePadre;
                    }
                }
            }
            catch (Exception ex)
            {
                string error = "Error al obtener el cliente agrupador " + ex.Message.ToString();

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);
                return new List<ClientePadre>();
            }
        }

        private static List<ClienteHijo> ObtenerClientesHijo(string sProductive, string sEmail)
        {
            string Universal = string.Empty;
            string DB = string.Empty;
            string StrSql = string.Empty;
            List<ClienteHijo> lsClienteHijo = new List<ClienteHijo>();

            try
            {
                // Obtenemos la informacion general del cliente
                using (var Con = new HanaConnection(Get_Parameterizations("HanaConec")))
                {
                    Con.Open();
                    DB = sProductive == "YES" ? Get_Parameterizations("DbSap") : Get_Parameterizations("DBNameTest");

                    StrSql = $@"SELECT ""CardCode"" ""CodigoCliente"", ""CardName"" ""NombreCliente"",
	                            ""LicTradNum"" ""RFC"", ""U_Almacen"" ""Almacen"", ""QryGroup2"" ""Credito""
	                            FROM {DB}.OCRD 
	                            WHERE LOWER (""E_Mail"") = '" + sEmail + "'";

                    // Se obtiene la informacion general del cliente hijo
                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClienteHijo oClienteHijo = new ClienteHijo();

                            //Datos generales
                            if (!string.IsNullOrEmpty(reader["CodigoCliente"].ToString()))
                                oClienteHijo.CodigoCliente = reader["CodigoCliente"].ToString();
                            if (!string.IsNullOrEmpty(reader["NombreCliente"].ToString()))
                                oClienteHijo.NombreCliente = reader["NombreCliente"].ToString();
                            if (!string.IsNullOrEmpty(reader["RFC"].ToString()))
                                oClienteHijo.RFC = reader["RFC"].ToString();
                            if (!string.IsNullOrEmpty(reader["Almacen"].ToString()))
                                oClienteHijo.Almacen = reader["Almacen"].ToString();
                            if (!string.IsNullOrEmpty(reader["Credito"].ToString()))
                            {
                                oClienteHijo.Credito = false;
                                if (reader["Credito"].ToString().Equals("Y"))
                                    oClienteHijo.Credito = true;
                            }

                            // Obtenemos la direccion Factura del cliente
                            oClienteHijo.DireccionFactura = ObtenerDireccionFactura(sProductive, oClienteHijo.CodigoCliente);

                            // Obtenemos las direcciones de Entrega del cliente
                            oClienteHijo.DireccionesEntrega = ObtenerDireccionEntrega(sProductive, oClienteHijo.CodigoCliente);

                            // Se agrega cada cliente hijo al proceso
                            lsClienteHijo.Add(oClienteHijo);

                        }
                    }

                    return lsClienteHijo;
                }
            }
            catch (Exception ex)
            {
                string error = "Error al obtener el cliente agrupador " + ex.Message.ToString();

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);
                return new List<ClienteHijo>();
            }
        }

        private static Direccion ObtenerDireccionFactura(string sProductive, object sCardCode)
        {
            string Universal = string.Empty;
            string DB = string.Empty;
            string StrSql = string.Empty;
            Direccion oDireccionFactura = new Direccion();

            try
            {
                // Obtenemos la informacion general del cliente
                using (var Con = new HanaConnection(Get_Parameterizations("HanaConec")))
                {
                    Con.Open();
                    DB = sProductive == "YES" ? Get_Parameterizations("DbSap") : Get_Parameterizations("DBNameTest");

                    StrSql = $@"SELECT ""Address"" ""CodigoDireccion"", ""Street"" ""Calle"", ""Block"" ""Colonia"", 
	                                ""ZipCode"" ""CP"", ""City"" ""Ciudad"", ""State"" ""Estado"", ""Country"" ""Pais""
                                FROM {DB}.CRD1 
                                WHERE ""AdresType"" = 'B' AND  ""CardCode"" = '" + sCardCode + "' ";

                    // Se obtiene la informacion general del cliente hijo
                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            //Datos generales
                            if (!string.IsNullOrEmpty(reader["CodigoDireccion"].ToString()))
                                oDireccionFactura.CodigoDireccion = reader["CodigoDireccion"].ToString();
                            if (!string.IsNullOrEmpty(reader["Calle"].ToString()))
                                oDireccionFactura.Calle = reader["Calle"].ToString();
                            if (!string.IsNullOrEmpty(reader["Colonia"].ToString()))
                                oDireccionFactura.Colonia = reader["Colonia"].ToString();
                            if (!string.IsNullOrEmpty(reader["CP"].ToString()))
                                oDireccionFactura.CP = reader["CP"].ToString();
                            if (!string.IsNullOrEmpty(reader["Ciudad"].ToString()))
                                oDireccionFactura.Ciudad = reader["Ciudad"].ToString();
                            if (!string.IsNullOrEmpty(reader["Estado"].ToString()))
                                oDireccionFactura.Estado = reader["Estado"].ToString();
                            if (!string.IsNullOrEmpty(reader["Pais"].ToString()))
                                oDireccionFactura.Pais = reader["Pais"].ToString();
                        }
                    }
                }
                return oDireccionFactura;
            }
            catch (Exception ex)
            {
                string error = "Error al obtener informacion de direccion del cliente " + ex.Message.ToString();

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);
                return new Direccion();
            }
        }

        private static List<Direccion> ObtenerDireccionEntrega(string sProductive, object sCardCode)
        {
            string Universal = string.Empty;
            string DB = string.Empty;
            string StrSql = string.Empty;
            List<Direccion> lsDireccionesEntrega = new List<Direccion>();

            try
            {
                // Obtenemos la informacion general del cliente
                using (var Con = new HanaConnection(Get_Parameterizations("HanaConec")))
                {
                    Con.Open();
                    DB = sProductive == "YES" ? Get_Parameterizations("DbSap") : Get_Parameterizations("DBNameTest");

                    StrSql = $@"SELECT ""Address"" ""CodigoDireccion"", ""Street"" ""Calle"", ""Block"" ""Colonia"", 
	                                ""ZipCode"" ""CP"", ""City"" ""Ciudad"", ""State"" ""Estado"", ""Country"" ""Pais""
                                FROM {DB}.CRD1 
                                WHERE ""AdresType"" != 'B' AND ""CardCode"" = '" + sCardCode + "' ";

                    // Se obtiene la informacion general del cliente hijo
                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Direccion oDireccionEntrega = new Direccion();
                            //Datos generales
                            if (!string.IsNullOrEmpty(reader["CodigoDireccion"].ToString()))
                                oDireccionEntrega.CodigoDireccion = reader["CodigoDireccion"].ToString();
                            if (!string.IsNullOrEmpty(reader["Calle"].ToString()))
                                oDireccionEntrega.Calle = reader["Calle"].ToString();
                            if (!string.IsNullOrEmpty(reader["Colonia"].ToString()))
                                oDireccionEntrega.Colonia = reader["Colonia"].ToString();
                            if (!string.IsNullOrEmpty(reader["CP"].ToString()))
                                oDireccionEntrega.CP = reader["CP"].ToString();
                            if (!string.IsNullOrEmpty(reader["Ciudad"].ToString()))
                                oDireccionEntrega.Ciudad = reader["Ciudad"].ToString();
                            if (!string.IsNullOrEmpty(reader["Estado"].ToString()))
                                oDireccionEntrega.Estado = reader["Estado"].ToString();
                            if (!string.IsNullOrEmpty(reader["Pais"].ToString()))
                                oDireccionEntrega.Pais = reader["Pais"].ToString();

                            lsDireccionesEntrega.Add(oDireccionEntrega);
                        }
                    }
                }
                return lsDireccionesEntrega;
            }
            catch (Exception ex)
            {
                string error = "Error al obtener informacion de direccion del cliente " + ex.Message.ToString();

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);
                return new List<Direccion>();
            }
        }

        internal static List<ArticulosStock> ConsultaStocks(string sProductive)
        {
            string Universal = string.Empty;
            string DB = string.Empty;
            string StrSql = string.Empty;
            List<ArticulosStock> lsArticulosStock = new List<ArticulosStock>();

            try
            {
                // Obtenemos la informacion general del cliente
                using (var Con = new HanaConnection(Get_Parameterizations("HanaConec")))
                {
                    Con.Open();
                    DB = sProductive == "YES" ? Get_Parameterizations("DbSap") : Get_Parameterizations("DBNameTest");

                    StrSql = $@"CALL {DB}.MP_ECOMMERCE_CONSULTA_STOCK";

                    // Se obtiene la informacion general del cliente hijo
                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArticulosStock oArticulo = new ArticulosStock();
                            //Datos generales
                            if (!string.IsNullOrEmpty(reader["ItemCode"].ToString()))
                                oArticulo.CodigoSAP = reader["ItemCode"].ToString();
                            if (!string.IsNullOrEmpty(reader["CodeBars"].ToString()))
                                oArticulo.SKU = reader["CodeBars"].ToString();


                            //Datos a por stock normal
                            List<StockPorAlmacen> lsStock = new List<StockPorAlmacen>();
                            StockPorAlmacen oStockArticulo = new StockPorAlmacen();      
                            if(!string.IsNullOrEmpty(reader["DisponibleAlmacen01NO"].ToString()))
                            {
                                oStockArticulo = new StockPorAlmacen();
                                oStockArticulo.CodigoAlmacen = "01";
                                oStockArticulo.Cantidad = Convert.ToInt32(reader["DisponibleAlmacen01NO"]);
                                lsStock.Add(oStockArticulo);
                            }
                            if (!string.IsNullOrEmpty(reader["DisponibleAlmacen02NO"].ToString()))
                            {
                                oStockArticulo = new StockPorAlmacen();
                                oStockArticulo.CodigoAlmacen = "02";
                                oStockArticulo.Cantidad = Convert.ToInt32(reader["DisponibleAlmacen02NO"]);
                                lsStock.Add(oStockArticulo);
                            }
                            if (!string.IsNullOrEmpty(reader["DisponibleAlmacen03NO"].ToString()))
                            {
                                oStockArticulo = new StockPorAlmacen();
                                oStockArticulo.CodigoAlmacen = "03";
                                oStockArticulo.Cantidad = Convert.ToInt32(reader["DisponibleAlmacen03NO"]);
                                lsStock.Add(oStockArticulo);
                            }
                            if (!string.IsNullOrEmpty(reader["DisponibleAlmacen05NO"].ToString()))
                            {
                                oStockArticulo = new StockPorAlmacen();
                                oStockArticulo.CodigoAlmacen = "05";
                                oStockArticulo.Cantidad = Convert.ToInt32(reader["DisponibleAlmacen05NO"]);
                                lsStock.Add(oStockArticulo);
                            }
                            oArticulo.Stock = lsStock;

                            //Datos a por stock Fecha corta
                            List<StockPorAlmacen> lsStockFechaCorta = new List<StockPorAlmacen>();
                            StockPorAlmacen oStockArticuloFechaCorta = new StockPorAlmacen();
                            if (!string.IsNullOrEmpty(reader["DisponibleAlmacen01SI"].ToString()) && Convert.ToInt32(reader["DisponibleAlmacen01SI"]) > 0)
                            {
                                oStockArticuloFechaCorta = new StockPorAlmacen();
                                oStockArticuloFechaCorta.CodigoAlmacen = "01";
                                oStockArticuloFechaCorta.Cantidad = Convert.ToInt32(reader["DisponibleAlmacen01SI"]);
                                lsStockFechaCorta.Add(oStockArticuloFechaCorta);
                            }
                            if (!string.IsNullOrEmpty(reader["DisponibleAlmacen02SI"].ToString()) && Convert.ToInt32(reader["DisponibleAlmacen02SI"]) > 0)
                            {
                                oStockArticuloFechaCorta = new StockPorAlmacen();
                                oStockArticuloFechaCorta.CodigoAlmacen = "02";
                                oStockArticuloFechaCorta.Cantidad = Convert.ToInt32(reader["DisponibleAlmacen02SI"]);
                                lsStockFechaCorta.Add(oStockArticuloFechaCorta);
                            }
                            if (!string.IsNullOrEmpty(reader["DisponibleAlmacen03SI"].ToString()) && Convert.ToInt32(reader["DisponibleAlmacen03SI"]) > 0)
                            {
                                oStockArticuloFechaCorta = new StockPorAlmacen();
                                oStockArticuloFechaCorta.CodigoAlmacen = "03";
                                oStockArticuloFechaCorta.Cantidad = Convert.ToInt32(reader["DisponibleAlmacen03SI"]);
                                lsStockFechaCorta.Add(oStockArticuloFechaCorta);
                            }
                            if (!string.IsNullOrEmpty(reader["DisponibleAlmacen05SI"].ToString()) && Convert.ToInt32(reader["DisponibleAlmacen05SI"]) > 0)
                            {
                                oStockArticuloFechaCorta = new StockPorAlmacen();
                                oStockArticuloFechaCorta.CodigoAlmacen = "05";
                                oStockArticuloFechaCorta.Cantidad = Convert.ToInt32(reader["DisponibleAlmacen05SI"]);
                                lsStockFechaCorta.Add(oStockArticuloFechaCorta);
                            }
                            oArticulo.StockFechaCorta = lsStockFechaCorta;

                            lsArticulosStock.Add(oArticulo);
                        }
                    }
                }
                return lsArticulosStock;
            }
            catch (Exception ex)
            {
                string error = "Error al obtener informacion del stock " + ex.Message.ToString();

                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                //Generar log
                Log(_path + @"\Configuracion\Log\" + fechaFormateada + ".txt", error);
                return new List<ArticulosStock>();
            }
        }

        #endregion

        #endregion
    }
}
