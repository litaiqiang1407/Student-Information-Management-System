using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

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

        public async Task<DataTable> GetData(string query)
        {
            DataTable dt = new DataTable();
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(SIMSConnection))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = await myCommand.ExecuteReaderAsync();
                    dt.Load(myReader);
                    myReader.Close();
                    await myCon.CloseAsync();
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
                    await myCon.CloseAsync();
                }
            }

            return rowsAffected;
        }
    }
}
