using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.DTOs
{
    public class UploadDocumentDto
    {
        [Required]
        public Guid UserId { get; set; }

       [Required]
        public IFormFile? PdfDocument { get; set; }

        [Required]
        public string? DocumentType { get; set; }
    }
}

