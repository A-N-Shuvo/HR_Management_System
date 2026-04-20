$(document).ready(function () {
    // আজকের তারিখ ডিফল্ট সেট করা
    $('#attDate').val(new Date().toISOString().split('T')[0]);
    loadAttendance();
});

function loadAttendance() {
    const date = $('#attDate').val();
    $.get('/Attendance/GetAttendanceData?date=' + date, function (res) {
        let rows = '';
        res.data.forEach(item => {
            rows += `<tr data-empid="${item.empId}">
                <td>${item.empCode}</td>
                <td>${item.empName}</td>
                <td><input type="time" class="form-control inTime" value="${item.attendance.inTime}"></td>
                <td><input type="time" class="form-control outTime" value="${item.attendance.outTime}"></td>
                <td>
                    <select class="form-control attStatus">
                        <option value="P" ${item.attendance.attStatus === 'P' ? 'selected' : ''}>Present</option>
                        <option value="A" ${item.attendance.attStatus === 'A' ? 'selected' : ''}>Absent</option>
                        <option value="L" ${item.attendance.attStatus === 'L' ? 'selected' : ''}>Late</option>
                    </select>
                </td>
            </tr>`;
        });
        $('#attTableBody').html(rows);
    });
}

function saveAttendance() {
    const attendanceList = [];
    const date = $('#attDate').val();

    $('#attTableBody tr').each(function () {
        attendanceList.push({
            empId: $(this).data('empid'),
            dtDate: date,
            inTime: $(this).find('.inTime').val(),
            outTime: $(this).find('.outTime').val(),
            attStatus: $(this).find('.attStatus').val()
        });
    });

    $.ajax({
        url: '/Attendance/SaveBulkAttendance',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(attendanceList),
        success: function (res) {
            if (res.success) {
                alert(res.message);
                loadAttendance();
            }
        }
    });
}