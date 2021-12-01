using Capital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capital.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {

        private readonly ILogger<EmployeeController> _logger;
        private readonly EmployeeContext employeeContext;
        /// <summary>
        /// CRUD Employee 
        /// </summary>
        public EmployeeController(ILogger<EmployeeController> logger, EmployeeContext employeeContext)
        {
            _logger = logger;
            this.employeeContext = employeeContext;
        }

        /// <summary>
        /// Display all the records
        /// </summary>
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            return employeeContext.GetAllRecord();
        }
        /// <summary>
        /// Insert Single Records
        /// </summary>
        /// <param name="emp"> employee object</param>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody]Employee emp)
        {
            try
            {
                _logger.LogInformation("Execution started");
                await employeeContext.InsertRecord(emp);
            }
            catch (MySqlException exp)  when (exp.ErrorCode == (int)MySqlErrorCode.DuplicateKeyEntry)
            {
                return BadRequest("Already Exist");
            }

            _logger.LogInformation("Execution Completed");
            return Ok("Success");
        }
        /// <summary>
        /// Insert Bulk Records
        /// </summary>
        /// <param name="emp"> employee object</param>
        [HttpPost("Bulk")]
        public async Task<ActionResult> BulkEntry([FromBody] List<Employee> emp)
        {
            try
            {
                _logger.LogInformation("bulk entry execution Started");
                await employeeContext.BulkInsert(emp);
            }
            catch (Exception exp)
            {
                _logger.LogWarning($"Exception occur durring bulk update {exp.InnerException}");
                throw;
            }
            _logger.LogInformation("bulk instertion Execution Completed");
            return Ok("Success");
        }
        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="status"> Active or Not</param>
        [HttpPost("BulkStatus")]
        public async Task<ActionResult> BulkUpdate([FromBody] bool status)
        {
            try
            {

                _logger.LogInformation("bulk Update Started");
                await employeeContext.UpdateStatus(status);
            }
            catch (Exception exp)
            {
                _logger.LogWarning($"Exception occur {exp.InnerException}");
                throw;
            }

            _logger.LogInformation("bulk update Completed");
            return Ok("Success");
        }
    }
}
