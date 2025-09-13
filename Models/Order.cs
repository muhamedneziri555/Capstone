using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarpetStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zip code is required")]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; } = "Pending";

        [Display(Name = "Card Number")]
        public string? CardNumber { get; set; }

        [Display(Name = "Card Holder Name")]
        public string? CardHolderName { get; set; }

        [Display(Name = "Expiry Date")]
        public string? ExpiryDate { get; set; }

        [Display(Name = "CVV")]
        public string? CVV { get; set; }

        public DateTime OrderDate { get; set; }
        
        [Required]
        public string OrderStatus { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal OrderTotal { get; set; }
        
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
