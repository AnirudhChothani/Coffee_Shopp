using ClosedXML.Excel;
using Coffee_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Coffee_Shop.Controllers
{
    public class ProductController : Controller

    {
        private IConfiguration configuration;

        public ProductController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region Excel Function
        public List<ProductModel> GetStudentModels()
        {
            List<ProductModel> productModels = new List<ProductModel>();
            string myconnStr = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(myconnStr);
            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SP_SelectAll_Product";
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ProductModel productModel = new ProductModel
                    {
                        
                        ProductName = reader["ProductName"].ToString(),
                        ProductPrice = reader["ProductPrice"] != DBNull.Value ? Convert.ToDecimal(reader["ProductPrice"]) : 0,
                        ProductCode = reader["ProductCode"].ToString(),
                        Description = reader["Description"].ToString(),
                        UserID = reader["UserID"] != DBNull.Value ? (int)reader["UserID"] : 0,
                                  // Add other properties as needed
                    };
                    productModels.Add(productModel);
                }
                return productModels;
            }
        }
        #endregion

        public IActionResult ExportStudentsToExcel()
        {

            List<ProductModel>productModels = GetStudentModels();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Product");
                // Add headers
                worksheet.Cell(1, 1).Value = "Product Name";
                worksheet.Cell(1, 2).Value = "ProductPrice";
                worksheet.Cell(1, 3).Value = "ProductCode";
                worksheet.Cell(1, 4).Value = "Description";
                worksheet.Cell(1, 5).Value = "User Name";

                // Add data
                int row = 2;
                foreach (var productModel in productModels)
                {
                    worksheet.Cell(row, 1).Value = productModel.ProductName;
                    worksheet.Cell(row, 2).Value = productModel.ProductPrice;
                    worksheet.Cell(row, 3).Value = productModel.ProductCode;
                   
                    worksheet.Cell(row, 4).Value = productModel.Description;
                    worksheet.Cell(row, 5).Value = productModel.UserID;

                    // Add other properties...
                    row++;
                }

                // Set content type and filename
                var contentType = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
                var fileName = "ProductData.xlsx";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }
        }


        #region Product
        public IActionResult Product()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_SelectAll_Product";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region Add-Edit
        public IActionResult AddEditProduct(int? productID)
        {
            UserDropDown();

            if (productID == null)
            {
                return View();
            }
            else
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand command = conn.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_SelectByID_Product";
                command.Parameters.AddWithValue("@ProductID", productID);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                ProductModel productModel = new ProductModel();

                foreach (DataRow dataRow in table.Rows)
                {
                    productModel.ProductID = Convert.ToInt32(@dataRow["ProductID"]);
                    productModel.ProductName = @dataRow["ProductName"].ToString();
                    productModel.ProductCode = @dataRow["ProductCode"].ToString();
                    productModel.ProductPrice = (decimal)Convert.ToDouble(@dataRow["ProductPrice"]);
                    productModel.Description = @dataRow["Description"].ToString();
                    productModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
                }
                return View("AddEditProduct", productModel);
            }
        }
        #endregion

        #region User Dropdown

        public void UserDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "SP_SelectAll_User";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<UserModel> userList = new List<UserModel>();
            foreach (DataRow use in dataTable1.Rows)
            {
                UserModel userModel = new UserModel();
                userModel.UserID = Convert.ToInt32(use["UserID"]);
                userModel.UserName = use["UserName"].ToString();
                userList.Add(userModel);
            }
            ViewBag.User = userList;
        }
        #endregion

        #region Save
        public IActionResult ProductSave(ProductModel productModel)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (ModelState.IsValid)
                {
                    if (productModel.ProductID == null)
                    {
                        command.CommandText = "SP_Insert_Product";
                        TempData["success"] = "Record Inserted Successfully!";
                    }
                    else
                    {
                        command.CommandText = "SP_UpdateById_Product";
                        command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productModel.ProductID;
                        TempData["success"] = "Record Updated Successfully!";
                    }

                    command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = productModel.ProductName;
                    command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = productModel.ProductPrice;
                    command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = productModel.ProductCode;
                    command.Parameters.Add("@Description", SqlDbType.VarChar).Value = productModel.Description;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = productModel.UserID;

                    command.ExecuteNonQuery();
                    return RedirectToAction("Product");
                }
                UserDropDown();
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Oops! Something Went Wrong!!";
                Console.WriteLine(ex.ToString());
            }
            return View("AddEditProduct", productModel);
        }
        #endregion

        #region Delete
        public IActionResult DeleteProduct(int productID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_DeleteById_Product";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productID;
                command.ExecuteNonQuery();

                TempData["delete"] = "Data Deleted Successfully!";
                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Data Cannot Be Deleted!";
                return RedirectToAction("Product");
            }
        }
        #endregion
    }
}

