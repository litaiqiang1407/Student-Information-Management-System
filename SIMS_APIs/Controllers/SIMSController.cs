using Microsoft.AspNetCore.Mvc;
using SIMS_APIs.Functions;
using System.Data;

namespace SIMS_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SIMSController : ControllerBase
    {
        private readonly DatabaseInteraction _dbInteraction;

        public SIMSController(IConfiguration configuration)
        {
            _dbInteraction = new DatabaseInteraction(configuration);
        }

        // Accounts
        [HttpGet]
        [Route("GetAccount")]
        public async Task<JsonResult> GetAccount()
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

            DataTable dt = await _dbInteraction.GetData(getAccountQuery);

            return new JsonResult(dt);
        }

        [HttpGet]
        [Route("GetRole")]
        public async Task<JsonResult> GetRoleAccount()
        {
            string getRoleAccountQuery = "SELECT " +
                                         "R.Name AS Role " +
                                         "FROM Role R";

            DataTable dt = await _dbInteraction.GetData(getRoleAccountQuery);

            List<string> roles = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                roles.Add(row["Role"].ToString());
            }

            return new JsonResult(roles);
        }

        // Get Account by Role
        //[HttpGet]
        //[Route("GetFilterAccount")]
        //public async Task<JsonResult> GetFilterAccount([FromForm] string role)
        //{
        //    string getFilterAccountQuery = "SELECT " +
        //                                   "A.MemberCode, " +
        //                                   "A.Email, " +
        //                                   "CONVERT(VARCHAR(10), A.CreatedAt, 103) AS CreatedAt, " +
        //                                   "CONVERT(VARCHAR(10), A.UpdatedAt, 103) AS UpdatedAt, " +
        //                                   "UI.Name AS Name, " +
        //                                   "R.Name AS Role " +
        //                                   "FROM Account A " +
        //                                   "LEFT JOIN UserInfo UI ON A.ID = UI.AccountID " +
        //                                   "LEFT JOIN UserRole UR ON A.ID = UR.AccountID " +
        //                                   "LEFT JOIN Role R ON UR.RoleID = R.ID " +
        //                                   "WHERE R.Name = @role";

        //    DataTable dt = await _dbInteraction.GetData(getFilterAccountQuery);

        //    return new JsonResult(dt);
        //}

        [HttpPost]
        [Route("AddAccount")]
        public async Task<JsonResult> AddAccount([FromForm] string newAccount)
        {
            string addAccountQuery = "INSERT INTO Account VALUES (@newAccount)";

            DataTable dt = await _dbInteraction.GetData(addAccountQuery);

            return new JsonResult("Add Successfully");
        }
    }
}
