using Assignment4_ServerSide_datatable.Models;
using Assignment4_ServerSide_datatable.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;

using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Assignment4_ServerSide_datatable.Controllers
{
    public class ADOEmployeeController : Controller
    {
        private static string connectionString = "";


        public ADOEmployeeController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("conStr");

        }

        public IActionResult Index()
        {

            return View();
        }
        public async Task<IActionResult> GetAllAsync()
        {
            IList<Employee> list = new List<Employee>();
            //string query = "SELECT * FROM EmployeeData";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(SD.Proc_GetAllEmployees))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                    {
                        while (sdr.Read())
                        {
                            list.Add(new Employee
                            {
                                Id = (int)sdr["Id"],
                                FirstName = (string)sdr["FirstName"],
                                LastName = (string)sdr["LastName"],
                                Address = (string)sdr["Address"],
                                Gender = (Gender)Enum.Parse(typeof(Gender), (string)sdr["Gender"]),
                                BirthDate = (DateTime)sdr["BirthDate"],
                            });
                        }
                    }
                    con.Close();
                }
            }
            return Json(new { data = list });



            //        SqlCommand cmd=new SqlCommand();
            //cmd.Connection = con;
            //cmd.CommandText = SD.Proc_GetAllEmployees;
            //cmd.CommandType = CommandType.StoredProcedure;

            //SqlDataReader dataReader= cmd.ExecuteReader();
            //if(dataReader.HasRows)
            //{

            //    Employee emp = new Employee()
            //    {
            //        Id = (int)dataReader["Id"],
            //        FirstName = dataReader["FirstName"].ToString(),
            //        LastName = dataReader["LastName"].ToString(),
            //        Address = dataReader["Address"].ToString(),
            //        Gender = (Gender)dataReader["Gender"],
            //        BirthDate = (DateTime)dataReader["BirthDate"],
            //    };
            //    //list.Add(new Employee
            //    //{
            //    //    Id = (int)dataReader["Id"],
            //    //    FirstName = dataReader["FirstName"].ToString(),
            //    //    LastName = dataReader["LastName"].ToString(),
            //    //    Address = dataReader["Address"].ToString(),
            //    //    Gender = (Gender)dataReader["Gender"],
            //    //    BirthDate = (DateTime)dataReader["BirthDate"],
            //    //});
            //}

            // return Json(new { data = list });
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Gender,Address,BirthDate")] Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {

                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(SD.Proc_CreateEmployee))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@firstname", employee.FirstName);
                            cmd.Parameters.AddWithValue("@lastname", employee.LastName);
                            cmd.Parameters.AddWithValue("@gender", employee.Gender);
                            cmd.Parameters.AddWithValue("@address", employee.Address);
                            cmd.Parameters.AddWithValue("@birthdate", employee.BirthDate);
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(employee);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(SD.Proc_DeleteEmployee))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Connection = con;
                        con.Open();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            con.Close();
                            return Json(new { success = true });

                        }
                        con.Close();
                        return Json(new { success = false });

                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {

                Employee employee = new Employee();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(SD.Proc_FindEmployee))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                employee.Id = (int)sdr["Id"];
                                employee.FirstName = (string)sdr["FirstName"];
                                employee.LastName = (string)sdr["LastName"];
                                employee.Address = (string)sdr["Address"];
                                employee.Gender = (Gender)Enum.Parse(typeof(Gender), (string)sdr["Gender"]);
                                employee.BirthDate = (DateTime)sdr["BirthDate"];
                            }

                        }
                        con.Close();
                    }
                }
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);

            }
            catch (Exception)
            {

                throw;
            }



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
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(SD.Proc_UpdateEmployee))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id", employee.Id);
                            cmd.Parameters.AddWithValue("@firtname", employee.FirstName);
                            cmd.Parameters.AddWithValue("@lastname", employee.LastName);
                            cmd.Parameters.AddWithValue("@gender", employee.Gender);
                            cmd.Parameters.AddWithValue("@address", employee.Address);
                            cmd.Parameters.AddWithValue("@birthdate", employee.BirthDate);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                }
                catch (DbUpdateConcurrencyException)
                {

                }

                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

    }
}
