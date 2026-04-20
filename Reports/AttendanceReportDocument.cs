using HR_Management_System.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
public class AttendanceReportDocument : IDocument
{
    private readonly List<AttendanceReportVM> _data;
    private readonly DateTime _fromDate;
    private readonly DateTime _toDate;

    public AttendanceReportDocument(List<AttendanceReportVM> data, DateTime from, DateTime to)
    {
        _data = data;
        _fromDate = from;
        _toDate = to;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Header().Text($"Attendance Report ({_fromDate:dd-MMM-yyyy} to {_toDate:dd-MMM-yyyy})").FontSize(20).SemiBold();

            page.Content().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Name
                    columns.RelativeColumn(1); // P
                    columns.RelativeColumn(1); // A
                    columns.RelativeColumn(1); // L
                });

                table.Header(header =>
                {
                    header.Cell().Text("Employee Name");
                    header.Cell().Text("Present");
                    header.Cell().Text("Absent");
                    header.Cell().Text("Late");
                });

                foreach (var item in _data)
                {
                    table.Cell().Text(item.EmpName);
                    table.Cell().Text(item.TotalPresent.ToString());
                    table.Cell().Text(item.TotalAbsent.ToString());
                    table.Cell().Text(item.TotalLate.ToString());
                }
            });
        });
    }
}