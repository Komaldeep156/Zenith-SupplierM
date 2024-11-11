using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IVacationRequests
    {
        Task<int> AddVacationRequests(VacationRequestsDTO model, string loggedInUserId);
        Task<List<VacationRequestsDTO>> GetVacationRequests();
        Task<VacationRequestsDTO> GetVacationRequestsId(int vacationRequestsId);
        Task<List<VacationRequestsDTO>> GetWorkBenchVacationRequests(DateTime filterStartDate, DateTime filterEndDate);
        Task<string> UpdateVacationRequests(VacationRequestsDTO model);
    }
}
