using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using ets.Models; 

namespace ets.Controllers
{
    [RoutePrefix("api/reg")]
    public class regController : ApiController
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString;

  
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Get()
        {
            List<AdminRegClass> registrations = new List<AdminRegClass>();
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cm = new SqlCommand("SP_GetAdminReg", cn) { CommandType = CommandType.StoredProcedure })
                {
                    await cn.OpenAsync();
                    using (var dr = await cm.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            registrations.Add(new AdminRegClass
                            {
                                ID = dr.GetInt32(dr.GetOrdinal("ID")),
                                OrganizationName = dr["OrganizationName"].ToString(),
                                FirstName = dr["FirstName"].ToString(),
                                LastName = dr["LastName"].ToString(),
                                Email = dr["Email"].ToString(),
                                PhoneNo = dr["PhoneNo"].ToString(),
                                Password = dr["Password"].ToString(),
                                ConfirmPassword = dr["ConfirmPassword"].ToString(),
                                DOB = dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"]) : (DateTime?)null,  // Handle null
                                Gender = dr["Gender"].ToString(),
                                City = dr["City"].ToString(),
                                Address = dr["Address"].ToString(),
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


        // GET api/reg/5
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            AdminRegClass registration = null;
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cm = new SqlCommand("SP_GetAdminById", cn) { CommandType = CommandType.StoredProcedure })
                {
                    cm.Parameters.AddWithValue("@ID", id);
                    await cn.OpenAsync();
                    using (var dr = await cm.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            registration = new AdminRegClass
                            {
                                ID = dr.GetInt32(dr.GetOrdinal("ID")),
                                OrganizationName = dr["OrganizationName"].ToString(),
                                FirstName = dr["FirstName"].ToString(),
                                LastName = dr["LastName"].ToString(),
                                Email = dr["Email"].ToString(),
                                PhoneNo = dr["PhoneNo"].ToString(),
                                Password = dr["Password"].ToString(),
                                ConfirmPassword = dr["ConfirmPassword"].ToString(),
                                DOB = dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"]) : (DateTime?)null,  // Handle null
                                Gender = dr["Gender"].ToString(),
                                City = dr["City"].ToString(),
                                Address = dr["Address"].ToString(),
                            };
                        }
                    }
                }

                if (registration != null)
                    return Ok(registration);
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return InternalServerError(ex);
            }
        }


        // POST api/reg
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Post([FromBody] AdminRegClass registration)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cm = new SqlCommand("SP_AddAdmin", cn) { CommandType = CommandType.StoredProcedure })
                {
                    cm.Parameters.AddWithValue("@OrganizationName", registration.OrganizationName);
                    cm.Parameters.AddWithValue("@FirstName", registration.FirstName);
                    cm.Parameters.AddWithValue("@LastName", registration.LastName);
                    cm.Parameters.AddWithValue("@Email", registration.Email);
                    cm.Parameters.AddWithValue("@PhoneNo", registration.PhoneNo);
                    cm.Parameters.AddWithValue("@Password", registration.Password); // Ideally, hash the password here
                    cm.Parameters.AddWithValue("@ConfirmPassword", registration.ConfirmPassword); // Hash this too
                    cm.Parameters.AddWithValue("@DOB", registration.DOB);
                    cm.Parameters.AddWithValue("@Gender", registration.Gender);
                    cm.Parameters.AddWithValue("@City", registration.City);
                    cm.Parameters.AddWithValue("@Address", registration.Address);

                    await cn.OpenAsync();
                    var result = await cm.ExecuteNonQueryAsync();

                    if (result > 0)
                        return Ok("Admin registered successfully!");
                    return BadRequest("Error inserting data.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return InternalServerError(ex);
            }
        }

        // DELETE api/reg/5
        [HttpDelete, Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cm = new SqlCommand("SP_DeleteAdmin", cn) { CommandType = CommandType.StoredProcedure })
                {
                    cm.Parameters.AddWithValue("@ID", id);
                    await cn.OpenAsync();
                    var result = await cm.ExecuteNonQueryAsync();

                    if (result > 0)
                        return Ok("Deleted successfully!");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return InternalServerError(ex);
            }
        }

        // PUT api/reg/5
        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Put(int id, [FromBody] AdminRegClass registration)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {
                using (var cn = new SqlConnection(_connectionString))
                using (var cm = new SqlCommand("SP_UpdateAdmin", cn) { CommandType = CommandType.StoredProcedure })
                {
                    cm.Parameters.AddWithValue("@ID", id);
                    cm.Parameters.AddWithValue("@OrganizationName", registration.OrganizationName);
                    cm.Parameters.AddWithValue("@FirstName", registration.FirstName);
                    cm.Parameters.AddWithValue("@LastName", registration.LastName);
                    cm.Parameters.AddWithValue("@Email", registration.Email);
                    cm.Parameters.AddWithValue("@PhoneNo", registration.PhoneNo);
                    cm.Parameters.AddWithValue("@Password", registration.Password); // Password should ideally be hashed
                    cm.Parameters.AddWithValue("@ConfirmPassword", registration.ConfirmPassword);
                    cm.Parameters.AddWithValue("@DOB", registration.DOB);
                    cm.Parameters.AddWithValue("@Gender", registration.Gender);
                    cm.Parameters.AddWithValue("@City", registration.City);
                    cm.Parameters.AddWithValue("@Address", registration.Address);

                    await cn.OpenAsync();
                    var result = await cm.ExecuteNonQueryAsync();

                    if (result > 0)
                        return Ok("Admin updated successfully!");
                    return NotFound(); // Return 404 if the record to update is not found
                }
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return InternalServerError(ex);
            }
        }

    }
}
