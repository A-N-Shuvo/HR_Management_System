$(document).ready(function () {
    loadEmployees();
    loadFormResources();
});

// ড্রপডাউন লোড করা (Dept, Desig, Shift)
function loadFormResources() {
    $.get('/Employee/GetFormResources', function (res) {
        let deptHtml = '<option value="">-- Select Dept --</option>';
        let desigHtml = '<option value="">-- Select Desig --</option>';
        let shiftHtml = '<option value="">-- Select Shift --</option>';

        res.departments.forEach(d => deptHtml += `<option value="${d.deptId}">${d.deptName}</option>`);
        res.designations.forEach(d => desigHtml += `<option value="${d.desigId}">${d.desigName}</option>`);
        res.shifts.forEach(s => shiftHtml += `<option value="${s.shifid}">${s.shiftName}</option>`);

        $('#deptId').html(deptHtml);
        $('#desigId').html(desigHtml);
        $('#shiftId').html(shiftHtml); // নিশ্চিত করুন HTML এ shiftId নামে select আছে
    });
}

// এমপ্লয়ি লিস্ট লোড
function loadEmployees() {
    $.get('/Employee/GetAll', function (res) {
        let rows = '';
        res.data.forEach(item => {
            rows += `<tr>
                <td>${item.empCode}</td>
                <td>${item.empName}</td>
                <td>${item.deptName}</td>  <td>${item.desigName}</td> <td>${item.gross.toFixed(2)}</td>
                <td>${item.dtJoin ? new Date(item.dtJoin).toLocaleDateString() : 'N/A'}</td>
                <td>
                    <button class="btn btn-sm btn-info" onclick="editEmp('${item.empId}')">Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="deleteEmp('${item.empId}')">Delete</button>
                </td>
            </tr>`;
        });
        $('#empTableBody').html(rows);
    });
}

// এডিট ফাংশন
function editEmp(id) {
    $.get('/Employee/GetAll', function (res) {
        const emp = res.data.find(e => e.empId === id);
        if (emp) {
            $('#empId').val(emp.empId);
            $('#empCode').val(emp.empCode);
            $('#empName').val(emp.empName);
            $('#deptId').val(emp.deptId); // সিলেক্ট বক্সে ভ্যালু সেট হবে
            $('#desigId').val(emp.desigId);
            $('#gross').val(emp.gross);
            // ডেট ফরম্যাট ফিক্স (YYYY-MM-DD)
            if (emp.dtJoin) {
                $('#dtJoin').val(new Date(emp.dtJoin).toISOString().split('T')[0]);
            }
            $('#empModal').modal('show');
        }
    });
}

// ডিলিট ফাংশন
function deleteEmp(id) {
    if (confirm("Are you sure you want to delete this employee?")) {
        $.ajax({
            url: '/Employee/Delete/' + id,
            type: 'DELETE',
            success: function (res) {
                if (res.success) {
                    alert(res.message);
                    loadEmployees();
                } else {
                    alert(res.message);
                }
            }
        });
    }
}

// এমপ্লয়ি সেভ করা
function saveEmployee() {
    const payload = {
        empId: $('#empId').val() || "00000000-0000-0000-0000-000000000000",
        empCode: $('#empCode').val(),
        empName: $('#empName').val(),
        deptId: $('#deptId').val(),
        desigId: $('#desigId').val(),
        shiftId: $('#shiftId').val() || null, // ShiftId যুক্ত করা হয়েছে
        gender: $('#gender').val(),
        gross: parseFloat($('#gross').val()) || 0,
        dtJoin: $('#dtJoin').val()
    };

    $.ajax({
        url: '/Employee/Upsert',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (res) {
            if (res.success) {
                $('#empModal').modal('hide');
                loadEmployees();
                alert(res.message);
                $('#empForm')[0].reset();
            } else {
                alert("Error: " + res.message);
            }
        },
        error: function (err) {
            alert("Something went wrong!");
        }
    });
}

function showEmpModal() {
    $('#empForm')[0].reset();
    $('#empId').val('');
    $('#empModal').modal('show');
}