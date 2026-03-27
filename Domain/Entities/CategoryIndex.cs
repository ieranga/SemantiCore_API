using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemantiCore_API.Domain.Entities
{
    public class CategoryIndex
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IndexType { get; set; } // 1=text, 2=message, 3=document

        [Required]
        [MaxLength(150)]
        public string IndexName { get; set; } = string.Empty;

        // Stores text/message OR document file name
        public string IndexValue { get; set; } = string.Empty;

        public int IndexCategoryId { get; set; }
        public IndexCategory IndexCategory { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
