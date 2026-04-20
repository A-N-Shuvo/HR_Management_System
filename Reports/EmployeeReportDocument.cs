using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HR_Management_System.ViewModels;

public class EmployeeReportDocument : IDocument
{
    private readonly List<EmployeeReportVM> _employees;

    public EmployeeReportDocument(List<EmployeeReportVM> employees)
    {
        _employees = employees ?? new List<EmployeeReportVM>(); // null হলে যেন এরর না দেয়
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(1, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10));

            // হেডার সেকশন
            page.Header().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Employee List Report").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                    col.Item().Text($"Date: {DateTime.Now:dd-MMM-yyyy}");
                });
            });

            // কন্টেন্ট বা টেবিল সেকশন
            page.Content().PaddingVertical(10).Table(table =>
            {
                // কলামগুলোর সাইজ নির্ধারণ (মোট ৬টি কলাম)
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50);  // Code
                    columns.RelativeColumn(2);   // Name
                    columns.RelativeColumn((float)1.5); // Join Date       
                    columns.RelativeColumn((float)1.5); // Dept
                    columns.RelativeColumn((float)1.5); // Desig
                    columns.RelativeColumn(1);   // Shift
                });

                // টেবিল হেডার
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Code");
                    header.Cell().Element(CellStyle).Text("Name");
                    header.Cell().Element(CellStyle).Text("Join Date");
                    header.Cell().Element(CellStyle).Text("Dept");
                    header.Cell().Element(CellStyle).Text("Designation");
                    header.Cell().Element(CellStyle).Text("Shift");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                // ডাটা লুপ
                foreach (var emp in _employees)
                {
                    table.Cell().Element(ValueStyle).Text(emp.EmpCode);
                    table.Cell().Element(ValueStyle).Text(emp.EmpName);
                    table.Cell().Element(ValueStyle).Text(emp.dtJoin);
                    table.Cell().Element(ValueStyle).Text(emp.DeptName);
                    table.Cell().Element(ValueStyle).Text(emp.DesigName);
                    table.Cell().Element(ValueStyle).Text(emp.ShiftName);

                    static IContainer ValueStyle(IContainer container)
                    {
                        return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    }
                }
            });

            // ফুটার সেকশন
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
        });
    }
}