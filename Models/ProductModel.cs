using System.ComponentModel.DataAnnotations;

namespace Coffee_Shop.Models
{
    public class ProductModel
    {
        public int? ProductID { get; set; }

        [Required(ErrorMessage = "Product Name Require")]
        [StringLength(20, ErrorMessage = "Must 20 character")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Price is Require")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Product Code Require")]
        [MinLength(5, ErrorMessage = "Minumum 5 char required !")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Product Description Require")]
        public string Description { get; set; }

        [Required(ErrorMessage = "User Require")]
        public int UserID { get; set; }
    }
}
