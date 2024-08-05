using SIMS.Shared.Functions;
using SIMS.Data.Entities;
using SIMS.Data.Entities.Admin;

namespace SIMS.Shared.Helpers
{
    public class DataService
    {
        private readonly DatabaseInteractionFunctions _databaseFunctions;

        public DataService(DatabaseInteractionFunctions databaseFunctions)
        {
            _databaseFunctions = databaseFunctions;
        }

        public async Task<List<Majors>> LoadMajorsAsync()
        {
            try
            {
                var majors = await _databaseFunctions.LoadData<Majors>("api/Admin/GetMajors");
                return majors.ToList(); // Chuyển đổi IEnumerable<Majors> thành List<Majors>
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading majors: {ex.Message}", ex);
            }
        }

        public async Task<List<Roles>> LoadRolesAsync()
        {
            try
            {
                var roles = await _databaseFunctions.LoadData<Roles>("api/Admin/GetRoles");
                return roles.ToList(); // Chuyển đổi IEnumerable<Roles> thành List<Roles>
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading roles: {ex.Message}", ex);
            }
        }
    }
}
