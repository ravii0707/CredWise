using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class AdminLoginDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

     [Required, MinLength(6)]
     public required string Password { get; set; }
    }
}
