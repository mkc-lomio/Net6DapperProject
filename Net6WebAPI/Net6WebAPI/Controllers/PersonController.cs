using Dapper;
using Microsoft.AspNetCore.Mvc;
using Net6WebAPI.ViewModels;
using System.Data.SqlClient;

namespace Net6WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        
        private readonly ILogger<PersonController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString = "";

        public PersonController(ILogger<PersonController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = _configuration["ConnectionString:SQLPlaygroundDB"]; 
        }


        [HttpGet("", Name = "GetPersons")]
        public async Task<ActionResult<PaginationViewModel<PersonViewModel>>> GetPersons(
             [FromQuery] int pageNumber,
             [FromQuery] int pageSize,
             [FromQuery] string? search,
             [FromQuery] string sortColumn = "Grade",
             [FromQuery] string sortType = "ASC"
            )
        {
            try
            {
                var sql = string.Format(@"EXEC spPaginatedPersons @pageNumber = {0}, @pageRows = {1},
@search = '{2}', @sortingColumn = '{3}', @sortingType = '{4}'",
pageNumber, pageSize, search ?? "", sortColumn, sortType);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    List<PersonViewModel> persons = new List<PersonViewModel>();
                    int count = 0;

                    using (var multi = await connection.QueryMultipleAsync(sql))
                    {
                        persons = multi.Read<PersonViewModel>().ToList();
                        count = multi.Read<int>().Single();
                    }

                    connection.Close();

                    return new PaginationViewModel<PersonViewModel>(pageNumber, pageSize, count, persons);
                }
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }


    }
}