namespace Net6WebAPI.ViewModels
{

    public class EmployeeTimeSheetReportViewModel
    {
        public List<EmployeeTimeSheetReportChildRowViewModel> Data { get; set; } = new List<EmployeeTimeSheetReportChildRowViewModel>();
        public decimal GrandTotal { get; set; }
    }

    public class EmployeeTimeSheetReportChildRowViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<EmployeeTimeSheetReportDateViewModel> Dates { get; set; } = new List<EmployeeTimeSheetReportDateViewModel>();
        public decimal Total { get; set; }
    }

    public class EmployeeTimeSheetReportDateViewModel
    {  
        public int IndexDay { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public decimal Hours { get; set; }
    }
}
