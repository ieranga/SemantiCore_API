using System.ComponentModel.DataAnnotations;

namespace SemantiCore_API.Domain.Entities
{
    public class IndexCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public ICollection<CategoryIndex> CategoryIndexes { get; set; } = new List<CategoryIndex>();
    }
}
