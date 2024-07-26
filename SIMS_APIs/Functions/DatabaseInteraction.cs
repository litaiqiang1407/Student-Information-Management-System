using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace SIMS_APIs.Functions
{
    public class DatabaseInteraction
    {
        private readonly IConfiguration _configuration;

        private string SIMSConnection => _configuration.GetConnectionString("SIMSConnection");

        public DatabaseInteraction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Method to get data without parameters
        public async Task<DataTable> GetData(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                    {
                        dt.Load(myReader);
                    }
                }
            }
            return dt;
        }

        // Overloaded method to get data with parameters
        public async Task<DataTable> GetData(string query, SqlParameter[] sqlParameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    if (sqlParameters != null)
                    {
                        myCommand.Parameters.AddRange(sqlParameters);
                    }

                    using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                    {
                        dt.Load(myReader);
                    }
                }
            }
            return dt;
        }

        public async Task<int> ExecuteQuery(string query, SqlParameter[] sqlParameters)
        {
            int rowsAffected;

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    if (sqlParameters != null)
                    {
                        myCommand.Parameters.AddRange(sqlParameters);
                    }

                    rowsAffected = await myCommand.ExecuteNonQueryAsync();
                }
            }

            return rowsAffected;
        }
    }
}
