using Coffee_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Coffee_Shop.Controllers
{
    public class BillController : Controller
    {
        private IConfiguration configuration;

        public BillController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region Bill
        public IActionResult Bill()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_SelectAll_Bills";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region Order Dropdown
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

        #region Add Edit Bill
        public IActionResult AddEditBill(int? billID)
        {
            OrderDropDown();
            UserDropDown();

            if (billID == null)
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
                command.CommandText = "SP_SelectById_Bills";
                command.Parameters.AddWithValue("@BillID", billID);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                BillModel billModel = new BillModel();

                foreach (DataRow dataRow in table.Rows)
                {
                    billModel.BillID = Convert.ToInt32(dataRow["BillID"]);
                    billModel.BillNumber = dataRow["BillNumber"].ToString();
                    billModel.BillDate = Convert.ToDateTime(dataRow["BillDate"]);
                    billModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                    billModel.TotalAmount = (decimal)Convert.ToDouble(dataRow["TotalAmount"]);
                    billModel.Discount = (decimal)Convert.ToDouble(dataRow["Discount"]);
                    billModel.NetAmount = (decimal)Convert.ToDouble(dataRow["NetAmount"]);
                    billModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                }
                return View("AddEditBill", billModel);
            }
        }
        #endregion

        #region Save Bill
        public IActionResult SaveBill(BillModel billModel)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (ModelState.IsValid)
            {
                if (billModel.BillID == null)
                {
                    command.CommandText = "SP_Insert_Bills";
                    TempData["success"] = "Record Inserted Successfully!";
                }
                else
                {
                    TempData["success"] = "Record Update Successfully!";
                    command.CommandText = "SP_UpdateById_Bills";
                    command.Parameters.Add("@BillID", SqlDbType.Int).Value = billModel.BillID;
                }

                command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billModel.BillNumber;
                command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = billModel.BillDate;
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = billModel.OrderID;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billModel.TotalAmount;
                command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billModel.Discount;
                command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billModel.NetAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = billModel.UserID;

                command.ExecuteNonQuery();
                return RedirectToAction("Bill");
            }

            OrderDropDown();
            UserDropDown();

            return View("AddEditBill", billModel);
        }
        #endregion

        #region Delete
        public IActionResult DeleteBill(int billID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_DeleteById_Bills";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = billID;
                command.ExecuteNonQuery();

                TempData["delete"] = "Data Deleted Successfully!";
                return RedirectToAction("Bill");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Data Cannot Be Deleted!";
                return RedirectToAction("Bill");
            }
        }
        #endregion
    }
}
