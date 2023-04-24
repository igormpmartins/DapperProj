using Dapper;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.Design;
using System.Data;
using System.Transactions;

namespace DapperDemo.Repository
{
    public class BonusRepository : IBonusRepository
    { 
        private IDbConnection db;

        public BonusRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void AddTestCompanyWithEmployees(Company company)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var sqlCompany = "INSERT INTO Companies (Name, Address, City, State, PostalCode)" +
                            " VALUES (@Name, @Address, @City, @State, @PostalCode)" +
                            " SELECT CAST(SCOPE_IDENTITY() as INT);";

                    var id = db.Query<int>(sqlCompany, company).Single();
                    company.CompanyId = id;

                    //foreach (var employee in company.Employees)
                    //{
                    //    employee.CompanyId = id;

                    //    var sqlComployee = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);" +
                    //        " SELECT CAST(SCOPE_IDENTITY() as INT);";

                    //    db.Execute(sqlComployee, employee);
                    //}

                    var sqlComployee = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);" +
                        " SELECT CAST(SCOPE_IDENTITY() as INT);";

                    company.Employees.Select(q => { q.CompanyId = id; return q; }).ToList();
                    db.Execute(sqlComployee, company.Employees);

                    transaction.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public List<Company> GetAllCompaniesWithEmployees()
        {
            var sql = "SELECT C.*, E.* FROM Employees E JOIN companies C ON C.CompanyId = E.CompanyId";

            var companyDic = new Dictionary<int, Company>();

            var companies = db.Query<Company, Employee, Company>(sql, (c, e) =>
            {
                if (!companyDic.TryGetValue(e.CompanyId, out Company currentCompany))
                {
                    currentCompany = c;
                    companyDic.Add(currentCompany.CompanyId, currentCompany);
                }

                currentCompany.Employees.Add(e);
                return currentCompany;

            }, splitOn: "EmployeeId");

            return companies.Distinct().ToList();
        }

        public Company GetCompanyWithEmployees(int id)
        {
            var parameters = new { CompanyId = id };

            var sql = "SELECT * FROM Companies WHERE CompanyId = @CompanyId;" +
                "SELECT * FROM Employees WHERE CompanyId = @CompanyId;";

            using var queries = db.QueryMultiple(sql, parameters);
            var company = queries.Read<Company>().ToList().FirstOrDefault();
            company.Employees = queries.Read<Employee>().ToList();

            return company;
        }

        public List<Employee> GetEmployeesWithCompany(int id)
        {
            var sql = "SELECT E.*, C.* FROM Employees E JOIN companies C ON C.CompanyId = E.CompanyId";
            if (id != 0)
                sql += " WHERE E.CompanyId = @id";

            //1-to-1 relationship
            var employees = db.Query<Employee, Company, Employee>(sql, (e, c) =>
            {
                e.Company = c;
                return e;
            }, new { id }, splitOn: "CompanyId");

            return employees.ToList();
        }

        public void RemoveRange(int[] companiesId)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId in @CompanyId";
            db.Query(sql, new { CompanyId = companiesId });
        }

        public List<Company> FilterCompanyByName(string name)
        {
            var sql = "SELECT * FROM Companies WHERE Name like '%' + @name + '%'";
            return db.Query<Company>(sql, new { name }).ToList();
        }
    }
}
