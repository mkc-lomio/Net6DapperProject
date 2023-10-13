using Dapper;
using Microsoft.AspNetCore.Mvc;
using Net6WebAPI.Helpers;
using Net6WebAPI.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;

namespace Net6WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        private readonly ILogger<ReportController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString = "";

        public ReportController(ILogger<ReportController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = "";
        }



        [HttpPost("employee-timesheet", Name = "GetEmployeeTimeSheetReport")]
        public async Task<ActionResult<EmployeeTimeSheetReportViewModel>> GetEmployeeTimeSheetReport(
      [FromBody] EmployeeTimeSheetReportParamViewModel employeeTimeSheet
          )
        {
            try
            {
                if(!DateHelper.IsValidDateRange(employeeTimeSheet.StartDate, employeeTimeSheet.EndDate)) //Date Range must have 16
                {
                    return BadRequest();
                }

                var proc = string.Format(@"[dbo].[kis_spGetEmployeeTimeSheetReport]");

                var param = new DynamicParameters();
                param.Add("@pEmployeeId", employeeTimeSheet.EmployeeId);
                param.Add("@pDateStart", "2023-10-01");
                param.Add("@pDateEnd", "2023-10-15");

                var result = await this.ExecuteSQL<dynamic>(proc, param);

                var modelMapper = new ModelMapper();
                var employeeTimeSheetReport = modelMapper.MapDynamicToEmployeeTimeSheetReportViewModel(result, employeeTimeSheet.StartDate, employeeTimeSheet.EndDate);

                return Ok(employeeTimeSheetReport);
            }
            catch
            {
                return NotFound();
            }
        }



        private async Task<IEnumerable<T>> ExecuteSQL<T>(string sql, DynamicParameters? parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync<T>(sql, parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
                connection.Dispose();
                return result;
            }
        }

        private async Task<int> ExecuteCUDSQL(string sql, DynamicParameters? parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    int result =  await SqlMapper.ExecuteAsync(connection, sql, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    if (result > 0)
                    {
                        transaction.Commit();

                    }
                    else
                    {
                       
                        transaction.Rollback();
                    }

                    connection.Close();
                    connection.Dispose();
                    return result;

                }
            }
        }

    }
}