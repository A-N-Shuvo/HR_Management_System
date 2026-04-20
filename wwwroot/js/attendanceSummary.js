$(document).ready(function () {
    // বর্তমান বছর এবং মাস ডিফল্ট সেট করা
    const now = new Date();
    $('#dtYear').val(now.getFullYear());
    $('#dtMonth').val(now.getMonth() + 1);
});

function loadSummary() {
    const year = $('#dtYear').val();
    const month = $('#dtMonth').val();

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
    });
}

function processSummary() {
    const year = $('#dtYear').val();
    const month = $('#dtMonth').val();

    if (!year || !month) {
        alert("Please select Year and Month");
        return;
    }

    $.post(`/AttendanceSummary/GenerateSummary?year=${year}&month=${month}`, function (res) {
        if (res.success) {
            alert(res.message);
            loadSummary();
        } else {
            alert("Error: " + res.message);
        }
    });
}