using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    /// <summary>
    /// EntityFramework model
    /// </summary>
    public class Contact
    {
        [Key]
        public int CID { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(48)")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(48)")]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(48)")]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR(24)")]
        public string Phone { get; set; }
    }
}

