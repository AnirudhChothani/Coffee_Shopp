using System.ComponentModel.DataAnnotations;

namespace Coffee_Shop.Models
{
    public class OrderModel
    {
        public int? OrderID { get; set; }

        [Required(ErrorMessage = "Date is Required")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        
        [Required(ErrorMessage = "Payment Mode is Invalid")]
        public string PaymentMode { get; set; }

        [Required(ErrorMessage = "Total Amount is Invalid")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Shipping Address is Requried")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "User Require")]
        public int UserID { get; set; }
    }
}