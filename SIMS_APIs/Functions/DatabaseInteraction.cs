using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace SIMS_APIs.Functions
{
    public class DatabaseInteraction
    {
        private readonly IConfiguration _configuration;

        public DatabaseInteraction(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DataTable> GetData(string query)
        {
            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("SIMSConnection");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
    }
}
