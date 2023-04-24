using DapperDemo.Models;

namespace DapperDemo.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmployeesWithCompany(int id);
        Company GetCompanyWithEmployees(int id);
        List<Company> GetAllCompaniesWithEmployees();
        void AddTestCompanyWithEmployees(Company company);
        void RemoveRange(int[] companiesId);
        List<Company> FilterCompanyByName(string name);
    }
}
