using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cors;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evaluacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        // GET: api/<UsuarioController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsuarioController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        private readonly IEmailSender _emailSender;
        private readonly EmailConfiguration _emailConfig;
        public UsuarioController(IEmailSender emailSender, EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
            _emailSender = emailSender;
        }
        // POST api/<UsuarioController>
        [EnableCors]
        [HttpPost]
        public string Post(IFormCollection formCollection)
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=green_leaves;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Dispose();
            List<Usuario> lst = new List<Usuario> { };
            Usuario respuesta = new Usuario();
            var validador = true;
            if (formCollection["nombre"].Contains(""))
            {
                respuesta.nombre = false;
                respuesta.dnombre = "ingrese su nombre";
                validador = false;
            
                
            }
            else {
                respuesta.nombre = true;
            }
            if (formCollection["email"].Contains("") )
            {
                respuesta.email = false;
                respuesta.demail = "Ingrese su email";
                
                validador = false;
            }
            else
            {
                if (!email_bien_escrito(formCollection["email"].ToString())) {
                    respuesta.email = false;
                    respuesta.demail = "email formato incorrecto";
                 
                    validador = false;


                } else {
                    respuesta.email = true;
                }
                
            }
            if (formCollection["telefono"].Contains(""))
            {
                respuesta.telefono =false;
                respuesta.dtelefono = "ingrese numero telefonico";
               
                validador = false;
            }
            else
            {
                if (!ValidarTelefonos7a10Digitos(formCollection["telefono"].ToString())) {
                    respuesta.telefono = false;
                    respuesta.dtelefono = "ingrese numero de telefono valido";
                 
                    validador = false;
                } else { respuesta.telefono = true; }
                
            }
            DateTime fecha1 = Convert.ToDateTime(DateTime.Now);
           

            DateTime fecha2 = Convert.ToDateTime(DateTime.Now.AddYears(-100));
            if (!formCollection["fecha"].Contains(""))
            {
                DateTime fecha = Convert.ToDateTime(formCollection["fecha"].ToString());

                if ((fecha >= fecha2) && (fecha <= fecha1))
                {
                    respuesta.fecha = true;

                }
                else
                {
                    respuesta.fecha = false;
                    respuesta.dfecha = "fecha no valida";

                    validador = false;

                }

            }
            else {
                respuesta.fecha = false;
                respuesta.dfecha = "ingrese la fecha";

                validador = false;
            }


            if (formCollection["estado"].Contains(""))
            {
                respuesta.ciudad = false;
                respuesta.dciudad = "ingrese la ciudad";
                
                validador = false;

            }
            else
            {
                respuesta.ciudad = true;
                respuesta.dciudad = formCollection["estado"];
            }


            if (validador)
            {
                conexion.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conexion;
                cmd.CommandText = "INSERT INTO usuario (`nombre`, `email`, `telefono`, `Fecha`,`id_estado`) VALUES (?nombre, ?email, ?telefono, ?fecha, ?estado);";
                cmd.Parameters.Add("?nombre", MySqlDbType.VarChar).Value = formCollection["nombre"].ToString();
                cmd.Parameters.Add("?email", MySqlDbType.VarChar).Value = formCollection["email"].ToString();
                cmd.Parameters.Add("?telefono", MySqlDbType.VarChar).Value = formCollection["telefono"].ToString();
                cmd.Parameters.Add("?fecha", MySqlDbType.DateTime).Value = Convert.ToDateTime(formCollection["fecha"]);
                cmd.Parameters.Add("?estado", MySqlDbType.VarChar).Value = formCollection["estado"].ToString();
                cmd.ExecuteNonQuery();
                respuesta.validador = true;

                var mensaje = "El usuario: "+ formCollection["nombre"].ToString()+" se contacto con los siguietnes datos: telefono:" + formCollection["telefono"].ToString()+" correo:"+ formCollection["email"].ToString()+" el dia"+ formCollection["fecha"].ToString();
                var message = new Message(new string[] { _emailConfig.ToSend }, "Nuevo contacto", mensaje);
                _emailSender.SendEmail(message);
            }
            else {

                respuesta.validador = false;
              }

            lst.Add(respuesta);
            return JsonSerializer.Serialize(lst);
        }

        // PUT api/<UsuarioController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsuarioController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private Boolean email_bien_escrito(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool ValidarTelefonos7a10Digitos(string strNumber)
        {
            Regex regex = new Regex(@"^[01]?[- .]?(\([1-9]\d{2}\)|[1-9]\d{2})[- .]?\d{3}[- .]?\d{4}$");
            Match match = regex.Match(strNumber);

            if (match.Success)
                return true;
            else
                return false;
        }
    }
}
