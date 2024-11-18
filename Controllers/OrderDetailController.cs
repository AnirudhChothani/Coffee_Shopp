using Coffee_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Coffee_Shop.Controllers
{
    public class OrderDetailController : Controller
    {
        private IConfiguration configuration;

        public OrderDetailController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region OrderDetail
        public IActionResult OrderDetail()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_SelectAll_OrderDetails";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region OrderDropDown
        public void OrderDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "SP_SelectAll_Order";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<OrderModel> orderList = new List<OrderModel>();
            foreach (DataRow order in dataTable1.Rows)
            {
                OrderModel orderModel = new OrderModel();
                orderModel.OrderID = Convert.ToInt32(order["OrderID"]);
                orderModel.CustomerName = order["CustomerName"].ToString();
                orderList.Add(orderModel);
            }
            ViewBag.Order = orderList;
        }
        #endregion

        #region Product Dropdown
        public void ProductDropDown()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "SP_SelectAll_Product";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<ProductModel> productList = new List<ProductModel>();
            foreach (DataRow product in dataTable1.Rows)
            {
                ProductModel productModel = new ProductModel();
                productModel.ProductID = Convert.ToInt32(product["ProductID"]);
                productModel.ProductName = product["ProductName"].ToString();
                productList.Add(productModel);
            }
            ViewBag.Product = productList;
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
            foreach (DataRow user in dataTable1.Rows)
            {
                UserModel userModel = new UserModel();
                userModel.UserID = Convert.ToInt32(user["UserID"]);
                userModel.UserName = user["UserName"].ToString();
                userList.Add(userModel);
            }
            ViewBag.User = userList;
        }
        #endregion

        #region Add-Edit
        public IActionResult AddEditOrderDetail(int? orderDetailID)
        {
            OrderDropDown();
            ProductDropDown();
            UserDropDown();

            if (orderDetailID == null)
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
                command.CommandText = "SP_SelectById_OrderDetail";
                command.Parameters.AddWithValue("@OrderDetailID", orderDetailID);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                OrderDetailModel orderDetailModel = new OrderDetailModel();

                foreach (DataRow dataRow in table.Rows)
                {
                    orderDetailModel.OrderDetailID = Convert.ToInt32(dataRow["OrderDetailID"]);
                    orderDetailModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                    orderDetailModel.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                    orderDetailModel.Quantity = Convert.ToInt32(dataRow["Quantity"]);
                    orderDetailModel.Amount = (decimal)Convert.ToDouble(dataRow["Amount"]);
                    orderDetailModel.TotalAmount = (decimal)Convert.ToDouble(dataRow["TotalAmount"]);
                    orderDetailModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                }
                return View("AddEditOrderDetail", orderDetailModel);
            }
        }
        #endregion

        #region Save
        public IActionResult SaveOrderDetail(OrderDetailModel orderDetailModel)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (ModelState.IsValid)
            {
                if (orderDetailModel.OrderDetailID == null)
                {
                    TempData["success"] = "Record Inserted Successfully!";
                    command.CommandText = "SP_Insert_OrderDetail";
                }
                else
                {
                    TempData["success"] = "Record Update Successfully!";
                    command.CommandText = "SP_Update_OrderDetail";
                    command.Parameters.Add("@OrderDetailID", SqlDbType.VarChar).Value = orderDetailModel.OrderDetailID;
                }

                command.Parameters.Add("@OrderID", SqlDbType.VarChar).Value = orderDetailModel.OrderID;
                command.Parameters.Add("@ProductID", SqlDbType.VarChar).Value = orderDetailModel.ProductID;
                command.Parameters.Add("@Quantity", SqlDbType.VarChar).Value = orderDetailModel.Quantity;
                command.Parameters.Add("@Amount", SqlDbType.VarChar).Value = orderDetailModel.Amount;
                command.Parameters.Add("@TotalAmount", SqlDbType.VarChar).Value = orderDetailModel.TotalAmount;
                command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = orderDetailModel.UserID;

                command.ExecuteNonQuery();
                return RedirectToAction("OrderDetail");
            }

            OrderDropDown();
            ProductDropDown();
            UserDropDown();

            return View("AddEditOrderDetail", orderDetailModel);

        }
        #endregion

        #region Delete
        public IActionResult DeleteOrderDetail(int orderDetailID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_DeleteById_OrderDetail";
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetailID;
                command.ExecuteNonQuery();

                TempData["delete"] = "Data Deleted Successfully!";
                return RedirectToAction("OrderDetail");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Data Cannot Be Deleted!";
                return RedirectToAction("OrderDetail");
            }
        }
        #endregion
    }
}
