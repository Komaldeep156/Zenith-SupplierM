namespace Zenith.BLL.DTO
{
    public class WorkbenchDTO
    {
        public string ApprovalType { get; set; }
        public int PendingStausCount { get; set; }
        public int WorkingStausCount { get; set; }
        public int TotalCount { get; set; }
        public string UserRole { get; set; }
        public int DelegateRequested { get; set; }
    }
}
