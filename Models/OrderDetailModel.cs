using System.ComponentModel.DataAnnotations;

namespace Coffee_Shop.Models
{
    public class OrderDetailModel
    {
        public int? OrderDetailID { get; set; }

        [Required]
        [Display(Name = "OrderID")]
        public int OrderID { get; set; }

        [Required]
        [Display(Name = "ProductID")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity  is Required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Amount  is Required")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "TotalAmount  is Required")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "User Required")]
        public int UserID { get; set; }
    }
}
