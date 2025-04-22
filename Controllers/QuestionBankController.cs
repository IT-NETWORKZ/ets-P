using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ets.Models;

namespace ets.Controllers
{
    public class QuestionBankController : ApiController
    {

        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString);
       
        public string Post(Questionbank questionbank)
        {
            string msg = "";
            if (questionbank != null)
            {
                SqlCommand cm = new SqlCommand("SP_AddCandidateRegister", cn);
                cm.CommandType = CommandType.StoredProcedure;
          
                cm.Parameters.AddWithValue("@TopicName", questionbank.TopicName ?? string.Empty);
                cm.Parameters.AddWithValue("@Difficulty", questionbank.Difficulty ?? string.Empty);
                cm.Parameters.AddWithValue("@Question", questionbank.Question);
                cm.Parameters.AddWithValue("@Option1", questionbank.Option1);
                cm.Parameters.AddWithValue("@Option2", questionbank.Option2);
                cm.Parameters.AddWithValue("@Option3", questionbank.Option3);
                cm.Parameters.AddWithValue("@Option4", questionbank.Option4);
                cm.Parameters.AddWithValue("@Option5", questionbank.Option5);
                cm.Parameters.AddWithValue("@Option6", questionbank.Option6);
                cm.Parameters.AddWithValue("@Option7", questionbank.Option7);
                cm.Parameters.AddWithValue("@Option8", questionbank.Option8);
                cm.Parameters.AddWithValue("@CorrectOption", questionbank.CorrectOption);
                cn.Open();
                try
                {
                    int m = cm.ExecuteNonQuery();
                    if (m > 0)
                    {
                        msg = "Question added Successful";
                    }
                    else
                    {
                        msg = "Failed to add question";
                    }
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
            }
            return msg;
        }

        //public CandidateReg Get(string sEmail, string sPassword)
        //{
        //    CandidateReg emp = null;
        //    SqlDataAdapter sda = new SqlDataAdapter("SP_BrowseCandidateRegUser", cn);
        //    sda.SelectCommand.CommandType = CommandType.StoredProcedure;
        //    sda.SelectCommand.Parameters.AddWithValue("@sEmail", sEmail);
        //    sda.SelectCommand.Parameters.AddWithValue("@sPassword", sPassword);

        //    DataTable dt = new DataTable();
        //    sda.Fill(dt);

        //    if (dt.Rows.Count > 0)
        //    {
        //        emp = new CandidateReg();
        //        emp.sEmail = dt.Rows[0]["sEmail"].ToString();
        //        emp.sPassword = dt.Rows[0]["sPassword"].ToString();
        //        // Add other fields if needed
        //    }
        //    CandidateReg user = Get(sEmail, sPassword);

        //    if (user != null)
        //    {
        //        Console.WriteLine("Login Success");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Not a valid user");
        //    }

        //    return emp; // returns null if user not found
        //}

    }
}
