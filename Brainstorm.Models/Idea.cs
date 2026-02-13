using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.Models
{
    public class Idea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public string? FilePath { get; set; } // Đường dẫn file upload (nếu có)

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // --- Thiết lập quan hệ với Category ---
        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; } // Khóa ngoại

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; } // Navigation Property

        // --- Thiết lập quan hệ với Topic ---
        [Required]
        [Display(Name = "Topic")]
        public int TopicId { get; set; } // Khóa ngoại

        [ForeignKey("TopicId")]
        public Topic? Topic { get; set; } // Navigation Property
    }
}
