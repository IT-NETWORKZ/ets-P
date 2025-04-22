using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ets.Models
{
	public class AdminRegClass
	{
        public int ID { get; set; }
        public string OrganizationName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // Nullable DateTime for DOB and Regdate to prevent conversion issues
        public DateTime? DOB { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public DateTime? Regdate { get; set; } // Nullable for flexibility




    }
}