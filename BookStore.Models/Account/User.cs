using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Account
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "First Name should contains alphabet only without any special characters or space.")]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "Last Name should contains alphabet only without any special characters or space.")]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50)]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(50)]
        public string Password { get; set; }        
        
        [Range(1,3,ErrorMessage ="Role is required.")]
        public int RoleId { get; set; }
        public Role RoleName { get; set; }
    }
}
