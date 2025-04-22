using ets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ets.Controllers
{
    [RoutePrefix("api/log")]
    public class logController : ApiController
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString;

        // GET api/reg
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Get()
        {
            List<AdminLog> registrations = new List<AdminLog>();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cm = new SqlCommand("SP_GetAdminLogin", cn) { CommandType = CommandType.StoredProcedure })
                {
                    await cn.OpenAsync();
                    using (var dr = await cm.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            registrations.Add(new AdminLog
                            {
                                ID = dr.GetInt32(dr.GetOrdinal("ID")),
                                Email = dr["Email"].ToString(),
                               
                                Password = dr["Password"].ToString()
                               
                            });
                        }
                    }
                }
                return Ok(registrations);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return InternalServerError(ex);
            }
        }

       [HttpGet, Route("{id:int}")]
public async Task<IHttpActionResult> Get(int id)
{
    AdminLog adminLog = null;

    try
    {
        using (var cn = new SqlConnection(_connectionString))
        using (var cm = new SqlCommand("SP_GetAdminloginById", cn) { CommandType = CommandType.StoredProcedure })
        {
            cm.Parameters.AddWithValue("@ID", id);
            await cn.OpenAsync();

            using (var dr = await cm.ExecuteReaderAsync())
            {
                if (await dr.ReadAsync())  // Check if a record is returned
                {
                    adminLog = new AdminLog
                    {
                        ID = dr.GetInt32(dr.GetOrdinal("ID")),
                        Email = dr["Email"].ToString(),
                        Password = dr["Password"].ToString()
                    };
                }
            }
        }

        // If a record was found, return it
        if (adminLog != null)
            return Ok(adminLog);

        // Return more detailed information for debugging
        return NotFound();  // If no record, return Not Found
    }
    catch (SqlException sqlEx)
    {
        return BadRequest($"SQL Error: {sqlEx.Message}");
    }
    catch (Exception ex)
    {
        return InternalServerError(ex);
    }
}
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Post([FromBody] AdminLog adminLog)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {
                using (var cn = new SqlConnection(_connectionString))
                {
                    await cn.OpenAsync();

                    using (var cm = new SqlCommand("SP_AdminLogin", cn) { CommandType = CommandType.StoredProcedure })
                    {
                        cm.Parameters.AddWithValue("@Email", adminLog.Email);
                        cm.Parameters.AddWithValue("@Password", adminLog.Password);

                        // Execute the DataReader and check for login success
                        using (var dr = await cm.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync()) // Login success
                            {
                                dr.Close(); // Close the reader before the next command

                                // Pass the necessary parameters (email, password, and connection)
                                await LogAdminEmail(adminLog.Email, adminLog.Password, cn);

                                return Ok(new { message = "Login successful!", email = adminLog.Email });
                            }
                            else if (await CheckEmailExists(adminLog.Email, cn))
                            {
                                dr.Close(); // Close the reader before returning Unauthorized
                                return Content(HttpStatusCode.Unauthorized, "Incorrect password.");
                            }
                            else
                            {
                                dr.Close(); // Close the reader before returning NotFound
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Modified LogAdminEmail method to insert email and password
        private async Task LogAdminEmail(string email, string password, SqlConnection cn)
        {
            // Insert Email and Password into AdminLogin table
            using (var logCmd = new SqlCommand("INSERT INTO AdminLogin (Email, Password) VALUES (@Email, @Password)", cn))
            {
                logCmd.Parameters.AddWithValue("@Email", email);
                logCmd.Parameters.AddWithValue("@Password", password);
                await logCmd.ExecuteNonQueryAsync();
            }
        }

        // Helper method to check if the email exists in AdminReg table
        private async Task<bool> CheckEmailExists(string email, SqlConnection cn)
        {
            using (var checkCmd = new SqlCommand("SELECT 1 FROM AdminReg WHERE Email = @Email", cn))
            {
                checkCmd.Parameters.AddWithValue("@Email", email);
                return await checkCmd.ExecuteScalarAsync() != null;
            }
        }






        // POST api/reg
        //[HttpPost, Route("")]
        //public async Task<IHttpActionResult> Post([FromBody] AdminLog adminLog)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest("Invalid data.");

        //    try
        //    {
        //        using (var cn = new SqlConnection(_connectionString))
        //        using (var cm = new SqlCommand("SP_AdminLogin", cn) { CommandType = CommandType.StoredProcedure })
        //        {
        //            cm.Parameters.AddWithValue("@Email", adminLog.Email);
        //            cm.Parameters.AddWithValue("@Password", adminLog.Password);

        //            await cn.OpenAsync();
        //            using (var dr = await cm.ExecuteReaderAsync())
        //            {
        //                if (await dr.ReadAsync()) // If a matching record is found
        //                {
        //                    return Ok("Login successful!");
        //                }
        //                else
        //                {
        //                    return Unauthorized(); // Return Unauthorized if no match is found
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);
        //    }
        //}
    }
}
