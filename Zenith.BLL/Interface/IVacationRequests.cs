using Zenith.BLL.DTO;

namespace Zenith.BLL.Interface
{
    public interface IVacationRequests
    {
        Task<int> AddVacationRequests(VacationRequestsDTO model, string loggedInUserId);
        Task<bool> CancelAllActiveVacationRequestsByUserId(string userId);
        Task<List<VacationRequestsDTO>> GetAccountVacationRequests(string userId, DateTime filterStartDate, DateTime filterEndDate);
        Task<List<VacationRequestsDTO>> GetVacationRequests();
        Task<VacationRequestsDTO> GetVacationRequestsId(Guid vacationRequestsId);
        Task<List<VacationRequestsDTO>> GetWorkBenchVacationRequests(DateTime filterStartDate, DateTime filterEndDate);
        Task<string> UpdateVacationRequests(VacationRequestsDTO model);
        Task<bool> UpdateVacationRequestsStatuses(List<string> rcrdIds, string status);
    }
}
