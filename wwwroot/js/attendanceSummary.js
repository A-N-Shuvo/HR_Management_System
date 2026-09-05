$(document).ready(function () {
    const now = new Date();
    $('#dtYear').val(now.getFullYear());
    $('#dtMonth').val(now.getMonth() + 1);
});

function loadSummary() {
    const year = $('#dtYear').val();
    const month = $('#dtMonth').val();

    if (!year || !month) {
        alert("Please select Year and Month");
        return;
    }

    $.get(`/AttendanceSummary/GetSummary?year=${year}&month=${month}`, function (res) {
        let rows = '';
        if (res.data && res.data.length > 0) {
            res.data.forEach(item => {
                rows += `<tr>
                    <td>${item.empCode}</td> <td>${item.empName}</td>
                    <td class="text-center">${item.present}</td>
                    <td class="text-center">${item.late}</td>
                    <td class="text-center">${item.absent}</td>
                    <td class="text-center">${item.period}</td>
                </tr>`;
            });
        } else {
            rows = '<tr><td colspan="6" class="text-center text-danger">No summary data found!</td></tr>';
        }
        $('#summaryTableBody').html(rows);
    }).fail(function (xhr) {
        var msg = xhr.responseJSON?.message || "Failed to load summary.";
        alert("Error: " + msg);
    });
}

function processSummary() {
    const year = $('#dtYear').val();
    const month = $('#dtMonth').val();

    if (!year || !month) {
        alert("Please select Year and Month");
        return;
    }

    if (!confirm("Generate attendance summary for " + year + "-" + month + "?")) return;

    $.ajax({
        url: '/AttendanceSummary/GenerateSummary',
        type: 'POST',
        data: { year: year, month: month },
        success: function (res) {
            if (res.success) {
                alert(res.message);
                loadSummary();
            } else {
                alert("Error: " + res.message);
            }
        },
        error: function (xhr) {
            var msg = xhr.responseJSON?.message || "Server error occurred.";
            alert("Error: " + msg);
        }
    });
}
