using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using cpDapper.Entidades;

namespace cpDapper.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase {

        private readonly string? _connectionString;
        public UsuarioController(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection OpenConnection()
        {
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, email, senha from Usuario; ";
            var result = await dbConnection.QueryAsync<Usuario>(sql);

            return Ok(result);
            
        }
        
        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id, nome, email, senha from Usuario where id = @id; ";
            var result = await dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new {id});

            dbConnection.Close();

            if (result == null) {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Usuario usuario)
        {
            using IDbConnection dbConnection = OpenConnection();
            string query = "INSERT INTO Usuario(nome, email, senha) VALUES(@Nome, @Email, @Senha); ";
        
            await dbConnection.ExecuteAsync(query, usuario);
            dbConnection.Close();

            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromBody] Usuario usuario)
        {

            using IDbConnection dbConnection = OpenConnection();

            // Atualiza o produto
            var query = @"UPDATE Usuario SET 
                          nome = @Nome,
                          email = @Email,
                          senha = @Senha
                          WHERE id = @Id";

            dbConnection.Execute(query, usuario);

            return Ok();
        }

        [HttpDelete("id")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection dbConnection = OpenConnection();

            var produto = await dbConnection.QueryAsync<Usuario>("DELETE FROM Usuario WHERE id = @id;", new { id });
            return Ok();
        }

    }
}
