using Coffee_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Coffee_Shop.Controllers
{
    public class OrderController : Controller
    {
        private IConfiguration configuration;

        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region Order
        public IActionResult Order()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_SelectAll_Order";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
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

        #region Add-Edit
        public IActionResult AddEditOrder(int? OrderID)
        {
            UserDropDown();
            if (OrderID == null)
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
                command.CommandText = "SP_SelectById_Order";
                command.Parameters.AddWithValue("@OrderID", OrderID);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                OrderModel orderModel = new OrderModel();

                foreach (DataRow dataRow in table.Rows)
                {
                    orderModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                    orderModel.OrderDate = Convert.ToDateTime(dataRow["OrderDate"]);
                    orderModel.CustomerName = dataRow["CustomerName"].ToString();
                    orderModel.PaymentMode = dataRow["PaymentMode"].ToString();
                    orderModel.TotalAmount = (decimal)Convert.ToDouble(dataRow["TotalAmount"]);
                    orderModel.ShippingAddress = dataRow["ShippingAddress"].ToString();
                    orderModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                }
                return View("AddEditOrder", orderModel);
            }

        }
        #endregion

        #region Save

        public IActionResult OrderSave(OrderModel orderModel)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (ModelState.IsValid)
            {
                if (orderModel.OrderID == null)
                {
                    TempData["success"] = "Record Inserted Successfully!";
                    command.CommandText = "SP_Insert_Order";
                }
                else
                {
                    TempData["success"] = "Record Update Successfully!";
                    command.CommandText = "SP_UpdateById_Order";
                    command.Parameters.Add("@OrderID", SqlDbType.VarChar).Value = orderModel.OrderID;
                }

                command.Parameters.Add("@OrderDate", SqlDbType.VarChar).Value = orderModel.OrderDate;
                command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = orderModel.CustomerName;
                command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
                command.Parameters.Add("@TotalAmount", SqlDbType.VarChar).Value = orderModel.TotalAmount;
                command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
                command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = orderModel.UserID;

                command.ExecuteNonQuery();
                return RedirectToAction("Order");
            }
            UserDropDown();
            return View("AddEditOrder", orderModel);
        }
        #endregion

        #region Delete

        public IActionResult DeleteOrder(int OrderID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_DeleteById_Order";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                command.ExecuteNonQuery();

                TempData["delete"] = "Data Deleted Successfully!";
                return RedirectToAction("Order");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Data Cannot Be Deleted!";
                return RedirectToAction("Order");
            }
        }
        #endregion
    }
}
