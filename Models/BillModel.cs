using System.ComponentModel.DataAnnotations;

namespace Coffee_Shop.Models
{
    public class BillModel
    {
        public int? BillID { get; set; }

        [Required]
        [Display(Name = "Bill Number")]
        public string BillNumber { get; set; }

        [Required]
        [Display(Name = "Bill Date")]
        public DateTime BillDate { get; set; }

        [Required(ErrorMessage = "OrderID is Required")]
        public int OrderID { get; set; }

        [Required]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Display(Name = "Discount")]
        public decimal? Discount { get; set; }

        [Required]
        [Display(Name = "NetAmount")]
        public decimal NetAmount { get; set; }

        [Required]
        [Display(Name = "User")]
        public int UserID { get; set; }
    }
}
