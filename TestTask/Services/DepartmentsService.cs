using System.Data.SqlClient;
using TestTask.Models;

namespace TestTask.Services
{
    public class DepartmentsService
    {
        private readonly string ConnectionString;

        public DepartmentsService(string connString)
        {
            ConnectionString = connString;
        }

        public async Task<List<Department>> GetAllDepartments()
        {
            var departments = new List<Department>();

            string sqlExpr = "SELECT ID,Code,Name,ParentDepartmentID FROM Department";
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);

                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return departments;
                }

                while (await reader.ReadAsync())
                {
                    Guid? parentId;
                    try { parentId = reader.GetGuid(3); }
                    catch { parentId = null; }

                    departments.Add(new Department()
                    {
                        ID = reader.GetGuid(0),
                        Code = Convert.ToString(reader.GetValue(1)),
                        Name = reader.GetString(2),
                        ParentDepartmentID = parentId,
                    });
                }
                reader.Close();
            }

            return departments;
        }

        public async Task<Department?> GetDepartment(Guid departmentId)
        {
            Department department;

            string sqlExpr = "SELECT D.ID,D.Code,D.Name,P.ID,P.Code,P.Name " +
                             "FROM Department D LEFT JOIN Department P ON D.ParentDepartmentID = P.ID " +
                             "WHERE D.ID=@id";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@id", departmentId));

                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return null;
                }

                await reader.ReadAsync();
                {
                    Guid? parentId;
                    try { parentId = reader.GetGuid(3); }
                    catch { parentId = null; }

                    department = new Department()
                    {
                        ID = reader.GetGuid(0),
                        Code = Convert.ToString(reader.GetValue(1)),
                        Name = reader.GetString(2),
                        ParentDepartmentID = parentId,
                    };

                    if (parentId != null)
                    {
                        department.ParentDepartment = new Department()
                        {
                            ID = (Guid)parentId,
                            Code = Convert.ToString(reader.GetValue(4)),
                            Name = reader.GetString(5),
                        };
                    }
                }
                reader.Close();
            }

            return department;
        }

        public async Task<bool> CheckDepartment(Guid departmentId)
        {
            string sqlExpr = "SELECT COUNT(ID) FROM Department WHERE ID=@id";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@id", departmentId));

                await conn.OpenAsync();
                return ((int)cmd.ExecuteScalar()) != 0;
            }
        }

        public async Task<List<Department>> GetChildDepartments(Guid departmentId)
        {
            var departments = new List<Department>();

            string sqlExpr = "SELECT ID,Code,Name FROM Department WHERE ParentDepartmentID=@parentId";
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@parentId", departmentId));

                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                {
                    reader.Close();
                    return departments;
                }

                while (await reader.ReadAsync())
                {
                    departments.Add(new Department()
                    {
                        ID = reader.GetGuid(0),
                        Code = Convert.ToString(reader.GetValue(1)),
                        Name = reader.GetString(2),
                    });
                }
                reader.Close();
            }

            return departments;
        }

        public async Task<bool> CheckChildDepartments(Guid departmentId)
        {
            string sqlExpr = "SELECT COUNT(ID) FROM Department WHERE ParentDepartmentID=@parentId";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@parentId", departmentId));

                await conn.OpenAsync();
                return ((int)cmd.ExecuteScalar()) != 0;
            }
        }

        public async Task<List<Employee>> GetDepartmentEmployees(Guid departmentId)
        {
            var employees = new List<Employee>();

            string sqlExpr = "SELECT ID,SurName,FirstName,Patronymic,Position FROM Empoyee WHERE DepartmentID=@departmentId";
            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@departmentId", departmentId));

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
                        DepartmentID = departmentId,
                    });
                }
                reader.Close();
            }

            return employees;
        }

        public async Task<bool> CheckDepartmentEmployees(Guid departmentId)
        {
            string sqlExpr = "SELECT COUNT(ID) FROM Empoyee WHERE DepartmentID=@departmentId";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@departmentId", departmentId));

                await conn.OpenAsync();
                return ((int)cmd.ExecuteScalar()) != 0;
            }
        }

        public async Task CreateDepartment(Department department)
        {
            string sqlExpr = "INSERT INTO Department (ID,Code,Name,ParentDepartmentID) " +
                             "VALUES (@id,@code,@name,@parentDepartmentID)";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@id", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@code", department.Code == null ? DBNull.Value : department.Code));
                cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                cmd.Parameters.Add(new SqlParameter("@parentDepartmentID", department.ParentDepartmentID == null ? DBNull.Value : department.ParentDepartmentID));

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateDepartment(Department department)
        {
            string sqlExpr = "UPDATE Department " +
                             "SET Code=@code, Name=@name " +
                             "WHERE ID=@id";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@id", department.ID));
                cmd.Parameters.Add(new SqlParameter("@code", department.Code == null ? DBNull.Value : department.Code));
                cmd.Parameters.Add(new SqlParameter("@name", department.Name));

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteDepartment(Guid departmentId)
        {
            string sqlExpr = "DELETE FROM Department WHERE ID=@id";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(sqlExpr, conn);
                cmd.Parameters.Add(new SqlParameter("@id", departmentId));

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
