using Dapper;
using Microsoft.AspNetCore.Mvc;
using Net6WebAPI.ViewModels;
using System.Data;
using System.Data.SqlClient;

namespace Net6WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeReimbursementController : ControllerBase
    {

        private readonly ILogger<EmployeeReimbursementController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString = "";

        public EmployeeReimbursementController(ILogger<EmployeeReimbursementController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = _configuration["ConnectionString:SQLPlaygroundDB"];
        }



        [HttpDelete("", Name = "DeleteEmployeeReimbursementById")]
        public async Task<ActionResult> DeleteEmployeeReimbursementById(
        [FromQuery] int id
            )
        {
            try
            {
             
               
                var proc = string.Format(@"kis_spEmployeeReimbursement_DeleteRecord");

                var param = new DynamicParameters();
                param.Add("@employeeReimbursementId", id);


                var result = await this.ExecuteCUDSQL(proc, param);


                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }



        [HttpPut("Cancellation", Name = "CancelEmployeeReimbursementById")]
        public async Task<ActionResult> CancelEmployeeReimbursementById(
        [FromQuery] int id
            )
        {
            try
            {


                var proc = string.Format(@"kis_spEmployeeReimbursement_CancellationRecord");

                var param = new DynamicParameters();
                param.Add("@employeeReimbursementId", id);


                var result = await this.ExecuteCUDSQL(proc, param);


                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }


        [HttpPost("", Name = "CreateUpdateEmployeeReimbursement")]
        public async Task<ActionResult> CreateUpdateEmployeeReimbursement(
      [FromBody] EmployeeReimbursementCreateUpdateViewModel employeeReimbursement
          )
        {
            try
            {


                var proc = string.Format(@"kis_spEmployeeReimbursement_UpdateInsertRecord");

                var param = new DynamicParameters();
                param.Add("@employeeReimbursementId", employeeReimbursement.Id);
                param.Add("@reimbursementTypeId", employeeReimbursement.ReimbursementTypeId);
                param.Add("@employeeId", employeeReimbursement.EmployeeId);
                param.Add("@reviewerEmployeeId", employeeReimbursement.ReviewerEmployeeId);
                param.Add("@reimbursementStatusId", employeeReimbursement.ReimbursementStatusId);
                param.Add("@additionalInfo", employeeReimbursement.AdditionalInfo);
                param.Add("@totalAmount", employeeReimbursement.TotalAmount);
                param.Add("@transactionDate", employeeReimbursement.TransactionDate); 
                param.Add("@approvedDate", employeeReimbursement.ApprovedDate);
                param.Add("@requestedDate", employeeReimbursement.RequestedDate); 
                param.Add("@reviewerRemarks", employeeReimbursement.ReviewerRemarks);
                param.Add("@modifiedBy", "system");
                param.Add("@createdBy", "system");
                param.Add("@isActive", employeeReimbursement.IsActive);
                param.Add("@dateCreated", DateTime.UtcNow);
                param.Add("@dateModified", DateTime.UtcNow);


                var result = await this.ExecuteCUDSQL(proc, param);


                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }


        [HttpPost("paginated-data", Name = "GetPaginatedEmployeeReimbursement")]
        public async Task<ActionResult<PaginationViewModel<EmployeeReimbursementPaginateViewModel>>> GetPaginatedEmployeeReimbursement(
       [FromBody] EmployeeReimbursementSearchViewModel searchViewModel
            )
        {
            try
            {
                int dataCount = 0;
                IEnumerable<EmployeeReimbursementPaginateViewModel> employeeReimbursements = new List<EmployeeReimbursementPaginateViewModel>();

                if (string.IsNullOrEmpty(searchViewModel.Search))
                {
                    var proc = string.Format(@"kis_spEmployeeReimbursementRetrieveAllCount");

                    var param = new DynamicParameters();
                    param.Add("@employeeId", searchViewModel.EmployeeId);

                    var result = await this.ExecuteSQL<int>(proc, param);
                    dataCount = result.ToList().FirstOrDefault();
                }
                else
                {
                    var proc = string.Format(@"kis_spEmployeeReimbursementRetrieveAllCount_Search");

                    var param = new DynamicParameters();
                    param.Add("@search", searchViewModel.Search);
                    param.Add("@employeeId", searchViewModel.EmployeeId);

                    var result = await this.ExecuteSQL<int>(proc, param);
                    dataCount = result.ToList().FirstOrDefault();
                }
                
                if (dataCount == 0) return NotFound();

                if (string.IsNullOrEmpty(searchViewModel.Search))
                {
                    var proc = string.Format(@"kis_spEmployeeReimbursementRetrieveAll_AutoGenByPage");

                    var param = new DynamicParameters();
                    param.Add("@employeeId", searchViewModel.EmployeeId);
                    param.Add("@pageNumber", searchViewModel.PageNumber);
                    param.Add("@pageRows", searchViewModel.PageSize);
                    param.Add("@sortingColumn", searchViewModel.SortColumn);
                    param.Add("@sortingType", searchViewModel.SortType);

                    employeeReimbursements = await this.ExecuteSQL<EmployeeReimbursementPaginateViewModel>(proc, param);
                }
                else
                {
                    var proc = string.Format(@"kis_spEmployeeReimbursementRetrieveAllBySearch_AutoGenByPage"); 
                    
                    var param = new DynamicParameters();
                    param.Add("@employeeId", searchViewModel.EmployeeId);
                    param.Add("@pageNumber", searchViewModel.PageNumber);
                    param.Add("@pageRows", searchViewModel.PageSize);
                    param.Add("@search", searchViewModel.Search);
                    param.Add("@sortingColumn", searchViewModel.SortColumn);
                    param.Add("@sortingType", searchViewModel.SortType);
           

                    employeeReimbursements = await this.ExecuteSQL<EmployeeReimbursementPaginateViewModel>(proc, param);
                }

                return new PaginationViewModel<EmployeeReimbursementPaginateViewModel>(searchViewModel.PageNumber, searchViewModel.PageSize, dataCount, employeeReimbursements);
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