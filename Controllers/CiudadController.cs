using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Cors;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Evaluacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CiudadController : ControllerBase
    {
        // GET: api/<CiudadController>
        [EnableCors]
        [HttpGet("{id}")]
        public IEnumerable<string> Get(string id)
        {
            string connectionString = "Server=127.0.0.1;Port=3306;Database=green_leaves;Uid=root;password=root;";
            MySqlConnection conexion = new MySqlConnection(connectionString);
            conexion.Dispose();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conexion;
            string str = $"SELECT countries.name AS pais, states.name AS estado,cities.name AS ciudad FROM countries LEFT JOIN states ON states.country_id = countries.id LEFT JOIN  cities ON cities.state_id = states.id WHERE cities.name LIKE '%" + id + "%' LIMIT 100";
            cmd.CommandText = str;
            conexion.Open();
         

            string [] ciudades = new string [100];
            using (var reader = cmd.ExecuteReader())
            {
                var i = 0;
                while (reader.Read())
                {
                    string  ciudad = ""+reader["ciudad"].ToString() + ", " + reader["estado"].ToString() + ", " + reader["pais"].ToString();
                    ciudades[i] = ciudad;
                    i++;
                }
                
            }
            conexion.Close();

            return ciudades;
        }

  

        // POST api/<CiudadController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CiudadController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CiudadController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
