using HR_Management_System.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace HR_Management_System.Reports
{
    public class SalarySummaryReportDocument : IDocument
    {
        private readonly List<SalaryReportVM> _data;
        private readonly int _month;
        private readonly int _year;
        private readonly string _deptName;

        public SalarySummaryReportDocument(List<SalaryReportVM> data, int month, int year, string deptName)
        {
            _data = data;
            _month = month;
            _year = year;
            _deptName = string.IsNullOrEmpty(deptName) ? "All Departments" : deptName;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4); // কলাম কম তাই পোর্ট্রেট মোডই যথেষ্ট
                page.Margin(40);

                // হেডার সেকশন
                page.Header().Column(column =>
                {
                    column.Item().Text("Salary Summary Report").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                    column.Item().Text(text =>
                    {
                        text.Span("Department: ").SemiBold();
                        text.Span(_deptName);
                    });
                    column.Item().Text(text =>
                    {
                        text.Span("Month: ").SemiBold();
                        text.Span($"{System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_month)}, {_year}");
                    });
                });

                page.Content().PaddingVertical(15).Table(table =>
                {
                    // ৩টি কলামের ডেফিনিশন (রিকোয়্যার্মেন্ট অনুযায়ী)
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4); // Employee Name
                        columns.RelativeColumn(2); // Total Salary (Gross)
                        columns.RelativeColumn(2); // Total Absent Amount
                    });

                    // টেবিল হেডার
                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("Employee Name");
                        header.Cell().Element(CellStyle).AlignRight().Text("Total Salary");
                        header.Cell().Element(CellStyle).AlignRight().Text("Total Absent Amt");

                        static IContainer CellStyle(IContainer container) =>
                            container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    });

                    // টেবিল ডাটা লুপ
                    foreach (var item in _data)
                    {
                        table.Cell().Element(ValueStyle).Text(item.EmpName ?? "N/A");
                        table.Cell().Element(ValueStyle).AlignRight().Text(item.Gross.ToString("N2"));
                        table.Cell().Element(ValueStyle).AlignRight().Text(item.AbsentAmount.ToString("N2"));
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        }

        private IContainer ValueStyle(IContainer container) =>
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).DefaultTextStyle(x => x.FontSize(11));
    }
}