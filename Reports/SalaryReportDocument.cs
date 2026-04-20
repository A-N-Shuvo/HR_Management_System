using HR_Management_System.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

public class SalaryReportDocument : IDocument
{
    private readonly List<SalaryReportVM> _data;
    private readonly int _month;
    private readonly int _year;

    public SalaryReportDocument(List<SalaryReportVM> data, int month, int year)
    {
        _data = data;
        _month = month;
        _year = year;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            // পেজ সাইজ ল্যান্ডস্কেপ করলে ৭টি কলাম সুন্দরভাবে ধরবে
            page.Size(PageSizes.A4.Landscape());
            page.Margin(30);

            // হেডার সেকশন
            page.Header().Text($"Monthly Salary Report - {System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_month)}, {_year}")
                .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            page.Content().PaddingVertical(10).Table(table =>
            {
                // ৭টি কলামের ডেফিনিশন
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2.5f); // Employee Name
                    columns.RelativeColumn(1.2f); // Gross
                    columns.RelativeColumn(1.2f); // Basic
                    columns.RelativeColumn(1f);   // HR
                    columns.RelativeColumn(1f);   // MA
                    columns.RelativeColumn(1.5f); // Absent Amount
                    columns.RelativeColumn(1.5f); // Payable Amount
                });

                // টেবিল হেডার
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Employee Name");
                    header.Cell().Element(CellStyle).AlignRight().Text("Gross");
                    header.Cell().Element(CellStyle).AlignRight().Text("Basic");
                    header.Cell().Element(CellStyle).AlignRight().Text("HR");
                    header.Cell().Element(CellStyle).AlignRight().Text("MA");
                    header.Cell().Element(CellStyle).AlignRight().Text("Abs. Amt");
                    header.Cell().Element(CellStyle).AlignRight().Text("Payable");

                    static IContainer CellStyle(IContainer container) =>
                        container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                });

                // টেবিল ডাটা লুপ
                foreach (var item in _data)
                {
                    table.Cell().Element(ValueStyle).Text(item.EmpName ?? "");
                    table.Cell().Element(ValueStyle).AlignRight().Text(item.Gross.ToString("N2"));
                    table.Cell().Element(ValueStyle).AlignRight().Text(item.Basic.ToString("N2"));
                    table.Cell().Element(ValueStyle).AlignRight().Text(item.Hrent.ToString("N2"));
                    table.Cell().Element(ValueStyle).AlignRight().Text(item.Medical.ToString("N2"));
                    table.Cell().Element(ValueStyle).AlignRight().Text(item.AbsentAmount.ToString("N2"));
                    table.Cell().Element(ValueStyle).AlignRight().Text(item.PayableAmount.ToString("N2"));
                }
            });

            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
            });
        });
    }

    // ValueStyle মেথডটি লুপের বাইরে এবং Compose মেথডের ভেতরে আলাদাভাবে রাখা হয়েছে
    private IContainer ValueStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .DefaultTextStyle(x => x.FontSize(10));
    }
}