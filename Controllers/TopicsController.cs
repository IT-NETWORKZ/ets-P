using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;
using System.Configuration;

namespace ets.Controllers
{
    public class TopicsController : ApiController
    {
        // POST: Add a new topic
        [HttpPost]
        [Route("api/topics")]
        public IHttpActionResult AddTopic([FromBody] TopicModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TopicName))
                return BadRequest("Topic name cannot be empty.");

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_AddTopic", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TopicName", model.TopicName);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new { success = true, message = "Topic added successfully." });
            }
            catch (SqlException ex)
            {
                return InternalServerError(new Exception("Database error: " + ex.Message));
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred: " + ex.Message));
            }
        }

        // GET: Fetch all topics
        [HttpGet]
        [Route("api/topics")]
        public IHttpActionResult GetTopics()
        {
            try
            {
                List<TopicModel> topics = new List<TopicModel>();
                string connectionString = ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT TopicId, TopicName FROM Topics ORDER BY TopicName"; // Added ordering by TopicName
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                topics.Add(new TopicModel
                                {
                                    TopicId = Convert.ToInt32(reader["TopicId"]),
                                    TopicName = reader["TopicName"].ToString()
                                });
                            }
                        }
                    }
                }

                if (topics.Count == 0)
                    return Ok(new { success = true, message = "No topics found.", data = topics });

                return Ok(topics);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: Update an existing topic by TopicId
        [HttpPut]
        [Route("api/topics/update/{id}")]
        public IHttpActionResult UpdateTopic(int id, [FromBody] TopicModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TopicName))
                return BadRequest("Topic name cannot be empty.");

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string updateQuery = "UPDATE Topics SET TopicName = @TopicName WHERE TopicId = @TopicId";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TopicId", id);
                        cmd.Parameters.AddWithValue("@TopicName", model.TopicName);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new { success = true, message = "Topic updated successfully." });
                        else
                            return Ok(new { success = false, message = "Topic not found or update failed." });
                    }
                }
            }
            catch (SqlException ex)
            {
                return InternalServerError(new Exception("Database error: " + ex.Message));
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred: " + ex.Message));
            }
        }

        // DELETE: Delete a topic by TopicId
        [HttpDelete]
        [Route("api/topics/delete/{id}")]
        public IHttpActionResult DeleteTopic(int id)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string deleteQuery = "DELETE FROM Topics WHERE TopicId = @TopicId";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TopicId", id);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new { success = true, message = "Topic deleted successfully." });
                        else
                            return Ok(new { success = false, message = "Topic not found." });
                    }
                }
            }
            catch (SqlException ex)
            {
                return InternalServerError(new Exception("Database error: " + ex.Message));
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("An error occurred: " + ex.Message));
            }
        }
    }

    // The TopicModel class remains unchanged
    public class TopicModel
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; }
    }
}
