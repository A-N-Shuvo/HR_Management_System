using Microsoft.Reporting.NETCore;

namespace HR_Management_System.Reports
{
    public static class RdlcReportHelper
    {
        public static byte[] RenderReport(
            string rdlcPath,
            string dataSetName,
            object dataSource,
            string reportType = "PDF",
            Dictionary<string, string>? parameters = null)
        {
            using var report = new LocalReport();
            report.ReportPath = rdlcPath;
            report.DataSources.Clear();
            report.DataSources.Add(new ReportDataSource(dataSetName, dataSource));
            report.Refresh();

            if (parameters != null && parameters.Count > 0)
            {
                var definedParams = report.GetParameters();
                var definedNames = new HashSet<string>(
                    definedParams.Select(p => p.Name),
                    StringComparer.OrdinalIgnoreCase);

                var safeParams = parameters
                    .Where(p => definedNames.Contains(p.Key))
                    .Select(p => new ReportParameter(p.Key, p.Value))
                    .ToList();

                if (safeParams.Count > 0)
                    report.SetParameters(safeParams);
            }

            var result = report.Render(
                reportType == "PDF" ? "PDF" : "EXCELOPENXML",
                null,
                out var mimeType,
                out var encoding,
                out var fileNameExtension,
                out var streams,
                out var warnings);

            return result;
        }
    }
}
