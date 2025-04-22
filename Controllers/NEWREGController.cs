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
using System.Threading.Tasks;

namespace ets.Controllers
{
    public class NEWREGController : ApiController
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyAPI_Conn"].ConnectionString);
        Reg emp = new Reg();

        [HttpPost, Route("api/areg")]
        public string Post(Reg employee)
        {
            string msg = "";
            if (employee != null)
            {
                SqlCommand cm = new SqlCommand("SP_AReg", cn);
                cm.CommandType = CommandType.StoredProcedure;
                cm.Parameters.AddWithValue("@FirstName", employee.FirstName);

                cn.Open();
                try
                {
                    int m = cm.ExecuteNonQuery();
                    if (m > 0)
                    {
                        msg = "Success";

                    }
                    else
                    {
                        msg = "Error";
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

    }
}
