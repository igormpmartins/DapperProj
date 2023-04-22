using DapperDemo.Data;
using DapperDemo.Models;

namespace DapperDemo.Repository
{
    public class CompanyRepositoryEF : ICompanyRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CompanyRepositoryEF(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Company Add(Company company)
        {
            dbContext.Add(company);
            dbContext.SaveChanges();
            return company;
        }

        public Company Find(int id)
        {
            return dbContext.Companies.Find(id);
        }

        public List<Company> GetAll()
        {
            return dbContext.Companies.ToList();
        }

        public void Remove(int id)
        {
            var company = dbContext.Companies.Find(id);
            dbContext.Remove(company);
            dbContext.SaveChanges();
        }

        public Company Update(Company company)
        {
            dbContext.Update(company);
            dbContext.SaveChanges();
            return company;
        }
    }
}
