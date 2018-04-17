using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AspNetMvc.QuickStart.Models
{
    public class Student
    {      
        public string Name { get; set; }
        public string Password { get; set; }

        public int Gender { get; set; }
        
        public string Major { get; set; }

        public DateTime EntranceDate { get; set; }
    }
}