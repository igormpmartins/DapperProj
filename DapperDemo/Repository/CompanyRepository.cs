﻿using Dapper;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperDemo.Repository
{
    public class CompanyRepository: ICompanyRepository
    {
        private IDbConnection db;

        public CompanyRepository(IConfiguration configuration)
        {
            db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public Company Add(Company company)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode)" +
                " VALUES (@Name, @Address, @City, @State, @PostalCode)" + 
                " SELECT CAST(SCOPE_IDENTITY() as INT);";

            /*var id = db.Query<int>(sql, new 
            { 
                company.Name,  
                company.Address,  
                company.City,  
                company.State,  
                company.PostalCode
            }).Single();*/

            var id = db.Query<int>(sql, company).Single();

            company.CompanyId = id;
            return company;
        }

        public Company Find(int id)
        {
            var sql = "SELECT * FROM Companies WHERE CompanyId = @Id";
            return db.Query<Company>(sql, new { id }).FirstOrDefault();
        }

        public List<Company> GetAll()
        {
            var sql = "SELECT * FROM Companies";
            return db.Query<Company>(sql).ToList();
        }

        public void Remove(int id)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId = @Id";
            db.Execute(sql, new { id });
        }

        public Company Update(Company company)
        {
            var sql = "UPDATE Companies SET " + 
                "Name = @Name, Address = @Address, City = @City, State = @State, PostalCode = @PostalCode " +
                "WHERE CompanyId = @CompanyId";

            db.Execute(sql, company);

            return company;
        }
    }
}
