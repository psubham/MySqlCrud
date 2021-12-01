using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Capital.Models
{
    public class EmployeeContext
    {
        public string ConnectionString { get; set; }
        private readonly int threshold = 5000;
        private readonly ILogger<EmployeeContext> _logger;

        public EmployeeContext(IOptions<Storage> option, ILogger<EmployeeContext> _logger)
        {
            this.ConnectionString = option.Value.ConnectionString;
            this._logger = _logger;
        }
        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
        public IEnumerable<Employee> GetAllRecord()
        {

            List<Employee> list = new List<Employee>();
            _logger.LogInformation("Fetching all records");
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"select * from Employee", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Employee()
                        {
                            Id = reader["id"].ToString(),
                            Name = reader["Name"].ToString(),
                            IsActive = bool.Parse(reader["IsActive"].ToString()),
                            PhoneNo = reader["Phone"].ToString(),
                            Email = reader["Email"].ToString()
                        });
                    }
                }
            }
            return list;
        }
        public async Task<int> InsertRecord(Employee emp)
        {
            int recordUpdate = 0;

            _logger.LogInformation($"Inserting  record {emp.Id}");
            string sqlQuery = $"Insert into Employee (id, Name ,IsActive, Phone, Email ) Values(@id,  @Name, @IsActive, @Phone, @Email)";
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                var command = new MySqlCommand(sqlQuery, conn);
                command.Parameters.AddWithValue("@id", emp.Id);
                command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(emp.Name) ? (object)DBNull.Value : emp.Name);
                command.Parameters.AddWithValue("@IsActive", emp.IsActive);
                command.Parameters.AddWithValue("@Phone", emp.PhoneNo);
                command.Parameters.AddWithValue("@Email", emp.Email);
                recordUpdate = await command.ExecuteNonQueryAsync();
            }
            return recordUpdate;
        }
        public async Task BulkInsert(List<Employee> employees)
        {

            _logger.LogInformation($"bulk insertion   record");
            StringBuilder sCommand = new StringBuilder("Insert into Employee (id, Name ,IsActive, Phone, Email ) Values");
            using (MySqlConnection mConnection =GetConnection())
            {
                List<string> Rows = new List<string>();
                foreach (var emp in employees)
                {
                    Rows.Add(string.Format("('{0}','{1}',{2},'{3}','{4}')", MySqlHelper.EscapeString(emp.Id), MySqlHelper.EscapeString(emp.Name),
                        emp.IsActive, MySqlHelper.EscapeString(emp.PhoneNo), MySqlHelper.EscapeString(emp.Email)));
                }
                sCommand.Append(string.Join(",", Rows));
                sCommand.Append(";");
                mConnection.Open();
                _logger.LogInformation($"bulk insertion   record cmd {sCommand.ToString()}");
                using (MySqlCommand myCmd = new MySqlCommand(sCommand.ToString(), mConnection))
                {
                    myCmd.CommandType = CommandType.Text;
                    await myCmd.ExecuteNonQueryAsync();
                }
            }

        }
        public async Task UpdateStatus(bool status)
        {
            bool oppoStatus = !status;
            string scommannd = $"UPDATE Employee e  INNER JOIN(SELECT e.id  FROM Employee e where e.IsActive <> {oppoStatus}  LIMIT {threshold}) b on e.id = b.id SET e.IsActive ={status} ";
            using (MySqlConnection mConnection = GetConnection())
            {
                int rowUpdate = 0;
                mConnection.Open();

                _logger.LogInformation($"bulk update   record cmd {scommannd}");
                using (MySqlCommand myCmd = new MySqlCommand(scommannd, mConnection))
                {
                    do
                    {
                        myCmd.CommandType = CommandType.Text;
                        rowUpdate = await myCmd.ExecuteNonQueryAsync();
                    } while (rowUpdate > 0);
                }
            }
        }
    }
}
