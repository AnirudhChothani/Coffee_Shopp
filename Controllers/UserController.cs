using Coffee_Shop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Coffee_Shop.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration configuration;

        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #region Login
        public IActionResult Login()
        {
            return View();
        }
        #endregion

        #region user Login
        [HttpPost]
        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "SP_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["notFound"] = "User not found";
                        return RedirectToAction("Login", "User");
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }
        #endregion


        #region log out

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        #endregion

        #region Register
        public IActionResult Register()
        {
           
                return View();
            
          
        }
        #endregion

        #region register save

        public IActionResult RegisterSave(UserModel userModel)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (ModelState.IsValid)
            {
                
                command.CommandText = "SP_Insert_User";
              

                command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userModel.UserName;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = userModel.Email;
                command.Parameters.Add("@Password", SqlDbType.VarChar).Value = userModel.Password;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userModel.MobileNo;
                command.Parameters.Add("@Address", SqlDbType.VarChar).Value = userModel.Address;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = userModel.IsActive;

                command.ExecuteNonQuery();
                return RedirectToAction("Login");
            }

            return View("Register",userModel);
        }
        #endregion

        #region User

        public IActionResult User()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "SP_SelectAll_User";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region Add-Edit
        public IActionResult AddEditUser(int? userID)
        {
            if (userID == null)
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
                command.CommandText = "SP_SelectById_User";
                command.Parameters.AddWithValue("@UserID", userID);
                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                UserModel userModel = new UserModel();

                foreach (DataRow dataRow in table.Rows)
                {
                    userModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                    userModel.UserName = dataRow["UserName"].ToString();
                    userModel.Email = dataRow["Email"].ToString();
                    userModel.Password = dataRow["Password"].ToString();
                    userModel.MobileNo = dataRow["MobileNo"].ToString();
                    userModel.Address = dataRow["Address"].ToString();
                    userModel.IsActive = (bool)(dataRow["IsActive"]);
                }



                return View("AddEditUser", userModel);  
            }
        }
        #endregion

        #region Save User
        public IActionResult UserSave(UserModel userModel)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (ModelState.IsValid)
            {
                if (userModel.UserID == null)
                {
                    TempData["success"] = "Record Inserted Successfully!";
                    command.CommandText = "SP_Insert_User";
                }
                else
                {
                    TempData["success"] = "Record Update Successfully!";
                    command.CommandText = "SP_UpdateById_User";
                    command.Parameters.Add("@UserID", SqlDbType.VarChar).Value = userModel.UserID;
                }

                command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userModel.UserName;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = userModel.Email;
                command.Parameters.Add("@Password", SqlDbType.VarChar).Value = userModel.Password;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userModel.MobileNo;
                command.Parameters.Add("@Address", SqlDbType.VarChar).Value = userModel.Address;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = userModel.IsActive;

                command.ExecuteNonQuery();
                return RedirectToAction("User");
            }

            return View("AddEditUser", userModel);   //?????????????????????????????????????????
        }
        #endregion

        #region Delete
        public IActionResult DeleteUser(int UserID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "SP_DeleteById_User";
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                command.ExecuteNonQuery();

                TempData["delete"] = "Data Deleted Successfully!";
                return RedirectToAction("User");
            }
            catch (Exception ex)
            {
                TempData["delete"] = "Data Cannot Be Deleted!";
                return RedirectToAction("User");
            }
        }
        #endregion
    }
}
