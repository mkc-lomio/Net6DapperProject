using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Net6WebAPI.ViewModels;

namespace Net6WebAPI.Helpers
{
    public class ModelMapper
    {

        public EmployeeTimeSheetReportViewModel? MapDynamicToEmployeeTimeSheetReportViewModel(IEnumerable<dynamic> dObject, DateTime startDate, DateTime endDate)
        {
                var parent = new EmployeeTimeSheetReportViewModel();
                var parentChild = new List<EmployeeTimeSheetReportChildRowViewModel>();
                var objectARR = dObject.ToArray();
                var dateSequence = DateHelper.GenerateDateSequence(startDate, endDate);
                var dateSequenceArr = dateSequence.ToArray();

                if (objectARR.Length == 0)
                    return null;

                for (int i = 0; i <= objectARR.Length - 1; i++)
                {
                    var dataRow = new EmployeeTimeSheetReportChildRowViewModel();
                    var dates = new List<EmployeeTimeSheetReportDateViewModel>();

                    var dataObjRow = objectARR[i] as IDictionary<string, object>;
                    if (dataObjRow != null && dataObjRow.ContainsKey("Id"))
                    {
                        dataRow.Id = Convert.ToInt32(dataObjRow["Id"]);
                    }

                    if (dataObjRow != null && dataObjRow.ContainsKey("Description"))
                    {
                        dataRow.Description = dataObjRow["Description"].ToString() ?? "";
                    }

                    if (dataObjRow != null && dataObjRow.ContainsKey("Total"))
                    {
                        dataRow.Total = (decimal)dataObjRow["Total"];

                        if (i == objectARR.Length - 1)
                        {
                            parent.GrandTotal = (decimal)dataObjRow["Total"];
                        }
                     }  

                    for (int j = 0; j <= dateSequenceArr.Length - 1; j++)
                    {
                        var strDate = dateSequenceArr[j];
                        if (dataObjRow != null && dataObjRow.ContainsKey(strDate))
                        {
                            dates.Add(new EmployeeTimeSheetReportDateViewModel()
                            {
                                IndexDay = j + 1,
                                Key = strDate,
                                Date = strDate,
                                Hours = (decimal)dataObjRow[strDate]
                            });
                        }
                    }

                    dataRow.Dates = dates;

                   

                    parentChild.Add(dataRow);
                }

                parent.Data = parentChild;

                return parent;
        }
    }
}
