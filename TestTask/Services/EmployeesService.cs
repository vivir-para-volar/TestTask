using System.Data.SqlClient;
using TestTask.Models;

namespace TestTask.Services
{
    public class EmployeesService
    {
        private readonly string ConnectionString;

        public EmployeesService(string connString)
        {
            ConnectionString = connString;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            var employees = new List<Employee>();

            string sqlExpr = "SELECT E.ID,E.SurName,E.FirstName,E.Patronymic,E.Position,D.ID,D.Code,D.Name " +
                             "FROM Empoyee E LEFT JOIN Department D ON E.DepartmentID = D.ID";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);

                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return employees;
                }

                while (await reader.ReadAsync())
                {
                    employees.Add(new Employee()
                    {
                        ID = Convert.ToInt32(reader.GetValue(0)),
                        SurName = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        Patronymic = Convert.ToString(reader.GetValue(3)),
                        Position = reader.GetString(4),
                        DepartmentID = reader.GetGuid(5),

                        Department = new Department()
                        {
                            ID = reader.GetGuid(5),
                            Code = Convert.ToString(reader.GetValue(6)),
                            Name = reader.GetString(7),
                        }
                    });
                }
                reader.Close();
            }

            return employees;
        }

        public async Task<Employee?> GetEmployee(int employeeId)
        {
            Employee employee;

            string sqlExpr = "SELECT E.ID,E.SurName,E.FirstName,E.Patronymic,E.DateOfBirth,E.DocSeries,E.DocNumber,E.Position,D.ID,D.Code,D.Name " +
                             "FROM Empoyee E LEFT JOIN Department D ON E.DepartmentID = D.ID " +
                             "WHERE E.ID=@id";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@id", employeeId));

                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return null;
                }

                await reader.ReadAsync();
                {
                    employee = new Employee()
                    {
                        ID = Convert.ToInt32(reader.GetValue(0)),
                        SurName = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        Patronymic = Convert.ToString(reader.GetValue(3)),
                        DateOfBirth = reader.GetDateTime(4),
                        DocSeries = Convert.ToString(reader.GetValue(5)),
                        DocNumber = Convert.ToString(reader.GetValue(6)),
                        Position = reader.GetString(7),
                        DepartmentID = reader.GetGuid(8),

                        Department = new Department()
                        {
                            ID = reader.GetGuid(8),
                            Code = Convert.ToString(reader.GetValue(9)),
                            Name = reader.GetString(10),
                        }
                    };
                }
                reader.Close();
            }

            return employee;
        }

        public async Task CreateEmployee(Employee employee)
        {
            string sqlExpr = "INSERT INTO Empoyee (SurName,FirstName,Patronymic,DateOfBirth,DocSeries,DocNumber,Position,DepartmentID) " +
                             "VALUES (@surName,@firstName,@patronymic,@dateOfBirth,@docSeries,@docNumber,@position,@departmentID)";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@surName", employee.SurName));
                cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                cmd.Parameters.Add(new SqlParameter("@patronymic", employee.Patronymic == null ? DBNull.Value : employee.Patronymic));
                cmd.Parameters.Add(new SqlParameter("@dateOfBirth", employee.DateOfBirth));
                cmd.Parameters.Add(new SqlParameter("@docSeries", employee.DocSeries == null ? DBNull.Value : employee.DocSeries));
                cmd.Parameters.Add(new SqlParameter("@docNumber", employee.DocNumber == null ? DBNull.Value : employee.DocNumber));
                cmd.Parameters.Add(new SqlParameter("@position", employee.Position));
                cmd.Parameters.Add(new SqlParameter("@departmentID", employee.DepartmentID));

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateEmployee(Employee employee)
        {
            string sqlExpr = "UPDATE Empoyee " +
                             "SET SurName=@surName, FirstName=@firstName, Patronymic=@patronymic, DateOfBirth=@dateOfBirth, " +
                             "    DocSeries=@docSeries, DocNumber=@docNumber, Position=@position, DepartmentID=@departmentID " +
                             "WHERE ID=@id";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@id", employee.ID));
                cmd.Parameters.Add(new SqlParameter("@surName", employee.SurName));
                cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                cmd.Parameters.Add(new SqlParameter("@patronymic", employee.Patronymic == null ? DBNull.Value : employee.Patronymic));
                cmd.Parameters.Add(new SqlParameter("@dateOfBirth", employee.DateOfBirth));
                cmd.Parameters.Add(new SqlParameter("@docSeries", employee.DocSeries == null ? DBNull.Value : employee.DocSeries));
                cmd.Parameters.Add(new SqlParameter("@docNumber", employee.DocNumber == null ? DBNull.Value : employee.DocNumber));
                cmd.Parameters.Add(new SqlParameter("@position", employee.Position));
                cmd.Parameters.Add(new SqlParameter("@departmentID", employee.DepartmentID));

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteEmployee(int employeeId)
        {
            string sqlExpr = "DELETE FROM Empoyee WHERE ID=@id";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@id", employeeId));

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
