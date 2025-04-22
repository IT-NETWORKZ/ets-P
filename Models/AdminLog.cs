using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ets.Models
{
	public class AdminLog
	{
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? Logdate { get; set; }
    }
}