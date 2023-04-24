using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DapperDemo.Data;
using DapperDemo.Models;
using DapperDemo.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DapperDemo.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ICompanyRepository companyRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBonusRepository bonusRepository;
        private readonly IDapperGenericRepository dapperGenericRepository;

        public CompaniesController(ICompanyRepository repository, IEmployeeRepository employeeRepository,
            IBonusRepository bonusRepository, IDapperGenericRepository dapperGenericRepository)
        {
            this.companyRepository = repository;
            this.employeeRepository = employeeRepository;
            this.bonusRepository = bonusRepository;
            this.dapperGenericRepository = dapperGenericRepository;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(companyRepository.GetAll());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var company = bonusRepository.GetCompanyWithEmployees(id.GetValueOrDefault());
            if (company == null)
                return NotFound();

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (ModelState.IsValid)
            {
                companyRepository.Add(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var company = companyRepository.Find(id.GetValueOrDefault());
            //var company = dapperGenericRepository.Single<Company>("usp_GetCompany", new { CompanyId = id.GetValueOrDefault() });
            if (company == null)
                return NotFound();

            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (id != company.CompanyId)
                return NotFound();

            if (ModelState.IsValid)
            {
                companyRepository.Update(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            companyRepository.Remove(id.GetValueOrDefault());

            return RedirectToAction(nameof(Index));
        }
    }
}
