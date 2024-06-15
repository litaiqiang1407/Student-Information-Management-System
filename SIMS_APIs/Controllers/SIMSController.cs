using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SIMS_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SIMSController : ControllerBase
    {
        private IConfiguration _configuration;

        public SIMSController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Accounts
        [HttpGet]
        [Route("GetAccount")]
        public JsonResult GetAccount()
        {
            string getAccountQuery = "SELECT " +
                                     "A.MemberCode, " +
                                     "A.Email, " +
                                     "CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, " +
                                     "CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, " +
                                     "UI.Name AS Name, " +
                                     "R.Name AS Role " +
                                     "FROM Account A " +
                                     "LEFT JOIN UserInfo UI ON A.ID = UI.AccountID " +
                                     "LEFT JOIN UserRole UR ON A.ID = UR.AccountID " +
                                     "LEFT JOIN Role R ON UR.RoleID = R.ID";

            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("SIMSConnection");
            SqlDataReader myReader;
            using(SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using(SqlCommand myCommand = new SqlCommand(getAccountQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(dt);
        }

        [HttpPost]
        [Route("AddAccount")]
        public JsonResult AddAccount([FromForm] string newAccount)
        {
            string addAccountQuery = "INSERT INTO Account VALUES (@newAccount)";

            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("SIMSConnection");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(addAccountQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@newAccount", newAccount);
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Add Successfully");
        }
    }
}
