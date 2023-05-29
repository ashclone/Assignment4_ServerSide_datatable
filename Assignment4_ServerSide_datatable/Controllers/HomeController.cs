using Assignment4_ServerSide_datatable.Data;
using Assignment4_ServerSide_datatable.Models;
using Assignment4_ServerSide_datatable.Utility;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;

namespace Assignment4_ServerSide_datatable.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private static string connectionString = "";



        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            connectionString = _context.Database.GetDbConnection().ConnectionString;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Test()
        {
            //var name = HttpContext.Request.Query["term"].ToString();
            //var firstname = _context.EmployeeData.Where(c => c.FirstName.Contains(name)).Select(c => c.FirstName).ToList();
            //return Ok(firstname);


            try
            {

                var name = HttpContext.Request.Query["term"].ToString() + '%';

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    using (SqlCommand cmd = new SqlCommand(SD.Proc_SearchFirstName))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        con.Open();
                        cmd.Parameters.AddWithValue("@text", name);
                       
                        using (SqlDataReader sdr =  cmd.ExecuteReader())
                        {
                            List<string> list = new List<string>();
                            while (sdr.Read())
                            {
                                list.Add((string)sdr["FirstName"]);                                  
                               
                            }
                           
                            return Ok(list);
                        }
                    }

                }
                //using (SqlConnection sqlCon = new SqlConnection(connectionString))
                //{
                //    DynamicParameters param = new();
                //    param.Add("@text ", name);

                //    sqlCon.Open();
                //    var data = sqlCon.Query(SD.Proc_SearchFirstName, param, commandType: CommandType.StoredProcedure);
                //    return Ok(data);
                //}
            }
            catch (Exception)
            {

                throw;
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}