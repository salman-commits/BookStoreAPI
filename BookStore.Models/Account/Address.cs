using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Account
{
    public class Address
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public bool IsPrimary { get; set; }

        [Required(ErrorMessage = "Address Line 1 is required")]
        [DataType(DataType.MultilineText)]
        [StringLength(200)]
        public string AddressLine1 { get; set; }
        
        [DataType(DataType.MultilineText)]
        [StringLength(200)]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(50)]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "City should contains alphabet only without any special characters or space.")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required")]
        [StringLength(50)]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "State should contains alphabet only without any special characters or space.")]
        public string State { get; set; }
        [Required(ErrorMessage = "Country is required")]
        [StringLength(50)]
        [RegularExpression(@"^[\p{L}]+$", ErrorMessage = "Country should contains alphabet only without any special characters or space.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression("^[0-9]{1,10}$", ErrorMessage = "Zip Code should contains number and not more than 10 digits")]
        public long ZipCode { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression("^[0-9]{1,10}$", ErrorMessage = "Phone Number should contains number and not more than 10 digits")]
        public long PhoneNumber { get; set; }
        public int AddressTypeId { get; set; }
        public AddressType AddressType { get; set; }
        
    }
}
