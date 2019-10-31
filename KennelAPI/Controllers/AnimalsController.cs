using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KennelAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace KennelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private string _connectionSring;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionSring);
            }
        }

        public AnimalsController(IConfiguration config)
        {
            _connectionSring = config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Breed, Age, HasShots
                                          FROM Animal";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Animal> animals = new List<Animal>();
                    while (reader.Read())
                    {
                        animals.Add(new Animal()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            HasShots = reader.GetBoolean(reader.GetOrdinal("HasShots"))
                        });
                    }

                    reader.Close();

                    return Ok(animals);
                }
            }
        }

        [HttpGet("{id}", Name = "GetAnimal")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Breed, Age, HasShots
                                          FROM Animal
                                         WHERE id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Animal animal = null;
                    if (reader.Read())
                    {
                        animal = new Animal()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            HasShots = reader.GetBoolean(reader.GetOrdinal("HasShots"))
                        };
                    }

                    reader.Close();

                    if (animal == null)
                    {
                        return NotFound();
                    }

                    return Ok(animal);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Animal newAnimal)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Animal (Name, Breed, Age, HasShots)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @breed, @age, @hasShots)";
                    cmd.Parameters.Add(new SqlParameter("@name", newAnimal.Name));
                    cmd.Parameters.Add(new SqlParameter("@breed", newAnimal.Breed));
                    cmd.Parameters.Add(new SqlParameter("@age", newAnimal.Age));
                    cmd.Parameters.Add(new SqlParameter("@hasShots", newAnimal.HasShots));

                    int newId = (int)cmd.ExecuteScalar();
                    newAnimal.Id = newId;
                    return CreatedAtRoute("GetAnimal", new { id = newId }, newAnimal);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Animal animal)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Animal
                                               SET Name = @name,
                                                   Breed = @breed,
                                                   Age = @age,
                                                   HasShots = @hasShots
                                             WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@name", animal.Name));
                        cmd.Parameters.Add(new SqlParameter("@breed", animal.Breed));
                        cmd.Parameters.Add(new SqlParameter("@age", animal.Age));
                        cmd.Parameters.Add(new SqlParameter("@hasShots", animal.HasShots));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Animal WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!AnimalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool AnimalExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" SELECT Id FROM Animal WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    bool doesExit = reader.Read();

                    reader.Close();

                    return doesExit;
                }
            }
        }
    }
}