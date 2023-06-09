﻿using System;
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
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly IBonusRepository bonusRepository;

        [BindProperty]
        public Employee Employee { get; set; }

        public EmployeesController(IEmployeeRepository employeeRepository, 
            ICompanyRepository companyRepository,
            IBonusRepository bonusRepository)
        {
            this.employeeRepository = employeeRepository;
            this.companyRepository = companyRepository;
            this.bonusRepository = bonusRepository;
        }

        public async Task<IActionResult> Index(int companyId = 0)
        {
            //var employees = employeeRepository.GetAll();
            //foreach (var employee in employees)
            //    employee.Company = companyRepository.Find(employee.CompanyId);

            var employees = bonusRepository.GetEmployeesWithCompany(companyId);
            return View(employees);
        }

        public IActionResult Create()
        {
            RetrieveCompanies();
            return View();
        }

        private void RetrieveCompanies()
        {
            var companiesList = companyRepository.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.CompanyId.ToString()
            });

            ViewBag.CompanyList = companiesList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePOST()
        {
            if (ModelState.IsValid)
            {
                //employeeRepository.Add(Employee);
                await employeeRepository.AddAsync(Employee);
                return RedirectToAction(nameof(Index));
            }

            RetrieveCompanies();
            return View(Employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            Employee = employeeRepository.Find(id.GetValueOrDefault());
            if (Employee == null)
                return NotFound();

            RetrieveCompanies();
            return View(Employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != Employee.EmployeeId)
                return NotFound();

            if (ModelState.IsValid)
            {
                employeeRepository.Update(Employee);
                return RedirectToAction(nameof(Index));
            }

            RetrieveCompanies();
            return View(Employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            employeeRepository.Remove(id.GetValueOrDefault());

            return RedirectToAction(nameof(Index));
        }
    }
}
