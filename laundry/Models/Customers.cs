using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace laundry.Models
{
    public class Customers
    {
        [Key]
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Full Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Email")]
        public DateTime FPick_Up_Date { get; set; }
        [DisplayName("Type Of Service")]
        public string Type_Of_Ser { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Phone No")]
        public string Phone_number { get; set; }
    }
}
