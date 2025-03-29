using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace elearn_server.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Role must be 3-20 characters")]
        public string RoleName { get; set; }
    }
}
