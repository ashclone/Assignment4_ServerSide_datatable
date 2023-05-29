using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment4_ServerSide_datatable.Data;
using Assignment4_ServerSide_datatable.Models;
using System.Linq.Expressions;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Assignment4_ServerSide_datatable.Utility;

namespace Assignment4_ServerSide_datatable.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static string connectionString = "";

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
            connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            //return _context.EmployeeData != null ?
            //            View(await _context.EmployeeData.ToListAsync()) :
            //            Problem("Entity set 'ApplicationDbContext.EmployeeData'  is null.");
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Getall()
        {
            try
            {
                int totalRecord = 0;
                int filterRecord = 0;
                var draw = Request.Form["draw"].FirstOrDefault();

                //var sortColumn = Request.Form["order[0][column]"].FirstOrDefault();

                var column_index = Request.Form["order[0][column]"].FirstOrDefault();
                var column_key = "columns[" + column_index + "][name]";
                var column_value = Request.Form[column_key].FirstOrDefault();

               // var col = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
               // var col3 = Request.Form["columns[" + 0 + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault() ?? "0";
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
                int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                // var data = await _context.EmployeeData.ToListAsync();
                IEnumerable<Employee> data;

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    await sqlCon.OpenAsync();
                  data = sqlCon.Query<Employee>(SD.Proc_GetAllEmployees,  commandType: CommandType.StoredProcedure);
                }

                //get total count of data in table
                totalRecord = data.Count();
                // search data when search value found
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    //data = data.Where(x => x.FirstName.ToLower().Contains(searchValue.ToLower()) || x.LastName.ToLower().Contains(searchValue.ToLower())).ToList();
                //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                //    {
                //        DynamicParameters param = new();
                //        param.Add("@SearchText", searchValue);
                //        param.Add("@DataSkip", skip);
                //        param.Add("@PageSize ", pageSize);
                //        await sqlCon.OpenAsync();
                //        data = sqlCon.Query<Employee>("SearchingAndSortingSP", param, commandType: CommandType.StoredProcedure);
                //    }
                //}
                // get total count of records after search
                //sort data
                filterRecord = data.Count();
                if (!(string.IsNullOrEmpty(searchValue)&&string.IsNullOrEmpty(column_value) && string.IsNullOrEmpty(sortColumnDirection)))
                {

                    // Sort the data based on the column name and direction

                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        DynamicParameters param = new();
                        param.Add("@DataSkip ", skip);
                        param.Add("@PageSize ", pageSize);
                        param.Add("@SearchText", searchValue);
                        param.Add("@sortColumn", column_value);
                        param.Add("@sortDirection", sortColumnDirection);
                        await sqlCon.OpenAsync();
                        data = sqlCon.Query<Employee>("SearchingAndSortingSP", param, commandType: CommandType.StoredProcedure);
                    }
                   // var property = typeof(Employee).GetProperty(column_value);
                    //if (sortColumnDirection.ToLower() == "asc")
                    //{
                    //    data = data.OrderBy(x => property.GetValue(x)).ToList();
                    //}
                    //else
                    //{
                    //   data=  data.OrderByDescending(x => property.GetValue(x)).ToList();
                    //}
                }               

                //pagination
                //var empList = data.Skip(skip).Take(pageSize).ToList();
                return Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecord,
                   recordsFiltered = filterRecord,
                    //data = empList
                    data=data
                });
            }
            catch (Exception)
            {

                throw;
            }

        }



        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Gender,Address,BirthDate")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                DynamicParameters param = new();
                param.Add("@firstname", employee.FirstName);
                param.Add("@lastname", employee.LastName);
                param.Add("@address", employee.Address);
                param.Add("@birthdate", employee.BirthDate);
                param.Add("@gender", employee.Gender);
                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    await sqlCon.OpenAsync();
                    sqlCon.Execute(SD.Proc_CreateEmployee, param, commandType: CommandType.StoredProcedure);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EmployeeData == null)
            {
                return NotFound();
            }

            var employee = await _context.EmployeeData.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);

        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Gender,Address,BirthDate")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(employee);
                    //await _context.SaveChangesAsync();
                    DynamicParameters param = new();
                    param.Add("@id", employee.Id);
                    param.Add("@firstname", employee.FirstName);
                    param.Add("@lastname", employee.LastName);
                    param.Add("@address", employee.Address);
                    param.Add("@birthdate", employee.BirthDate);
                    param.Add("@gender", employee.Gender);
                    using (SqlConnection sqlCon = new SqlConnection(connectionString))
                    {
                        await sqlCon.OpenAsync();
                        sqlCon.Execute(SD.Proc_UpdateEmployee, param, commandType: CommandType.StoredProcedure);
                    }
                    //return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                DynamicParameters param = new();
                param.Add("@id", id);

                using (SqlConnection sqlCon = new SqlConnection(connectionString))
                {
                    await sqlCon.OpenAsync();
                    sqlCon.Execute(SD.Proc_DeleteEmployee, param, commandType: CommandType.StoredProcedure);
                }
                return Json(new { success = true });
            }
            catch (Exception)
            {

                throw;
            }
           
           // if (_context.EmployeeData == null)
           // {
           //     return Problem("Entity set 'ApplicationDbContext.EmployeeData'  is null.");
           // }
           //var employee = await _context.EmployeeData.FindAsync(id);
           // if (employee != null)
           // {
           //     _context.EmployeeData.Remove(employee);
           //     await _context.SaveChangesAsync();
           //     return Json(new { success = true });
                


           // }
           // return Json(new { success = false });

        }

        private bool EmployeeExists(int id)
        {
            return (_context.EmployeeData?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
