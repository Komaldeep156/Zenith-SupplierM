using Zenith.BLL.DTO;
using Zenith.Repository.DomainModels;

namespace Zenith.BLL.Interface
{
    public interface IVacationRequests
    {
        /// <summary>
        /// Retrieves a list of vacation requests based on the assigned user ID.
        /// </summary>
        /// <param name="assignedUserId">The ID of the assigned user.</param>
        /// <returns>A list of vacation requests.</returns>
        Task<VacationRequestsDTO> GetVacationRequestsId(Guid vacationRequestsId);

        /// <summary>
        /// Retrieves a list of vacation requests for the workbench within a date range and for a specific user.
        /// </summary>
        /// <param name="filterStartDate">The start date for filtering vacation requests.</param>
        /// <param name="filterEndDate">The end date for filtering vacation requests.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>A list of vacation requests for the workbench.</returns>
        Task<List<VacationRequestsDTO>> GetWorkBenchVacationRequests(DateTime filterStartDate, DateTime filterEndDate, string loggedInUserId = default);

        /// <summary>
        /// Retrieves a list of vacation requests for a specific user within a date range.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="filterStartDate">The start date for filtering vacation requests.</param>
        /// <param name="filterEndDate">The end date for filtering vacation requests.</param>
        /// <returns>A list of vacation requests for the user.</returns>
        Task<List<VacationRequestsDTO>> GetAccountVacationRequests(string userId, DateTime filterStartDate, DateTime filterEndDate);

        /// <summary>
        /// Cancels all active vacation requests for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> CancelAllActiveVacationRequestsByUserId(string userId);

        /// <summary>
        /// Retrieves a vacation request by its ID.
        /// </summary>
        /// <param name="vacationRequestsId">The ID of the vacation request.</param>
        /// <returns>A vacation request DTO.</returns>
        Task<List<VacationRequestsDTO>> GetVacationRequests(string assignedUserId = default);

        /// <summary>
        /// Adds a new vacation request.
        /// </summary>
        /// <param name="model">The vacation request DTO.</param>
        /// <param name="loggedInUserId">The ID of the logged-in user.</param>
        /// <returns>An integer indicating the result of the operation.</returns>
        Task<int> AddVacationRequests(VacationRequestsDTO model, string loggedInUserId);

        /// <summary>
        /// Updates an existing vacation request.
        /// </summary>
        /// <param name="model">The vacation request DTO.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        Task<string> UpdateVacationRequests(VacationRequestsDTO model);

        /// <summary>
        /// Updates the statuses of multiple vacation requests.
        /// </summary>
        /// <param name="rcrdIds">A list of record IDs.</param>
        /// <param name="status">The new status.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> UpdateVacationRequestsStatuses(List<string> rcrdIds, string status);

        /// <summary>
        /// Updates the status of a specific vacation request.
        /// </summary>
        /// <param name="model">The vacation request model.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        Task<bool> UpdateVacationRequestStatus(VacationRequests model);
    }
}
