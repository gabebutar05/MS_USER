using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_USER.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public bool IsSuccess { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Status baris: 1 = aktif, 0 = non-aktif / soft delete
        /// </summary>
        public int RowStatus { get; set; } = 1;
    }
}
