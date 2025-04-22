using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ets.Models
{
	public class Questionbank
	{
        public int ID { get; set; }
        public string TopicName { get; set; }
        public string Difficulty { get; set; }
        public string Question { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Option5 { get; set; }
        public string Option6 { get; set; }
        public string Option7 { get; set; }
        public string Option8 { get; set; }
        public string CorrectOption { get; set; }
        public DateTime Regdate { get; set; }

    }
}