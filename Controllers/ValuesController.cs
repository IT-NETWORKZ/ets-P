using ets.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Http;

namespace ets.Controllers
{
    public class ValuesController : ApiController
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString);

        // GET: Retrieve all questions from the database
        public IEnumerable<Questionbank> Get()
        {
            List<Questionbank> questions = new List<Questionbank>();
            SqlCommand cm = new SqlCommand("SP_GetAllQuestions", cn);
            cm.CommandType = CommandType.StoredProcedure;
            cn.Open();
            SqlDataReader dr = cm.ExecuteReader();
            while (dr.Read())
            {
                questions.Add(new Questionbank
                {
                    ID = Convert.ToInt32(dr["ID"]),
                    Question = dr["Question"].ToString(),
                    Option1 = dr["Option1"].ToString(),
                    Option2 = dr["Option2"].ToString(),
                    Option3 = dr["Option3"].ToString(),
                    Option4 = dr["Option4"].ToString(),
                    Option5 = dr["Option5"].ToString(),
                    Option6 = dr["Option6"].ToString(),
                    Option7 = dr["Option7"].ToString(),
                    Option8 = dr["Option8"].ToString(),
                    CorrectOption = dr["CorrectOption"].ToString(),
                    TopicName = dr["TopicName"].ToString(),
                    Difficulty = dr["Difficulty"].ToString()
                });
            }
            cn.Close();
            return questions;
        }

        // POST: Handle text and media uploads
        [HttpPost]
        [Route("api/values/")]
        public IHttpActionResult UploadQuestion()
        {
            string msg = "";
            HttpRequest request = HttpContext.Current.Request;

            if (request.Form.Count > 0)  // Handle form data
            {
                string topicName = request.Form["TopicName"];
                string difficulty = request.Form["Difficulty"];
                string questionText = request.Form["QuestionText"];
                string correctOption = request.Form["CorrectOption"];

                string[] optionTexts = new string[8];
                string[] optionFilePaths = new string[8];

                for (int i = 0; i < 8; i++)
                {
                    optionTexts[i] = request.Form[$"Option{i + 1}Text"]; // Handle text-based option

                    // Handle file-based option (if uploaded)
                    if (request.Files[$"Option{i + 1}Media"] != null)
                    {
                        var file = request.Files[$"Option{i + 1}Media"];
                        optionFilePaths[i] = SaveUploadedFile(file);  // Save file and get file path
                    }
                }

                // Handle question media (image, audio, or video)
                string questionFilePath = null;
                if (request.Files["QuestionMedia"] != null)
                {
                    questionFilePath = SaveUploadedFile(request.Files["QuestionMedia"]);
                }

                // Call stored procedure to save question and options to the database
                SqlCommand cm = new SqlCommand("SP_AddCandidateRegister", cn);
                cm.CommandType = CommandType.StoredProcedure;

                // Pass parameters to the stored procedure
                cm.Parameters.AddWithValue("@TopicName", topicName);
                cm.Parameters.AddWithValue("@Difficulty", difficulty);
                cm.Parameters.AddWithValue("@QuestionText", questionText);
                cm.Parameters.AddWithValue("@QuestionFilePath", questionFilePath ?? (object)DBNull.Value);

                for (int i = 0; i < 8; i++)
                {
                    cm.Parameters.AddWithValue($"@Option{i + 1}Text", optionTexts[i]);
                    cm.Parameters.AddWithValue($"@Option{i + 1}FilePath", optionFilePaths[i] ?? (object)DBNull.Value);
                }

                cm.Parameters.AddWithValue("@CorrectOption", correctOption);

                cn.Open();
                try
                {
                    int result = cm.ExecuteNonQuery();
                    msg = result > 0 ? "Question added successfully" : "Failed to add question";
                }
                catch (Exception ex)
                {
                    return BadRequest("Error: " + ex.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
            else
            {
                return BadRequest("No form data received");
            }

            return Ok(msg);
        }

        // Utility method to save uploaded files to server
        private string SaveUploadedFile(HttpPostedFile file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileName(file.FileName);
                string filePath = HttpContext.Current.Server.MapPath("~/Uploads/" + fileName);

                file.SaveAs(filePath);  // Save file to server

                return "" + fileName;  // Return relative file path
            }
            return null;
        }
    }
}
