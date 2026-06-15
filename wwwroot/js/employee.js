$(document).ready(function () {
    // ১. প্রথমে ড্রপডাউন রিসোর্স লোড করব, তারপর এমপ্লয়ি লিস্ট লোড করব
    loadFormResources(function() {
        loadEmployees();
    });
});

// ড্রপডাউন লোড করা (Callback ফাংশন যুক্ত করা হয়েছে)
function loadFormResources(callback) {
    $.get('/Employee/GetFormResources', function (res) {
        let deptHtml = '<option value="">-- Select Dept --</option>';
        let desigHtml = '<option value="">-- Select Desig --</option>';
        let shiftHtml = '<option value="">-- Select Shift --</option>';

        res.departments.forEach(d => deptHtml += `<option value="${d.deptId}">${d.deptName}</option>`);
        res.designations.forEach(d => desigHtml += `<option value="${d.desigId}">${d.desigName}</option>`);
        res.shifts.forEach(s => shiftHtml += `<option value="${s.shifid}">${s.shiftName}</option>`);

        $('#deptId').html(deptHtml);
        $('#desigId').html(desigHtml);
        $('#shiftId').html(shiftHtml); 

        // ড্রপডাউন এইচটিএমএল তৈরি শেষ হলে এই কলব্যাক রান করবে
        if (typeof callback === "function") {
            callback();
        }
    });
}

// এমপ্লয়ি লিস্ট লোড
function loadEmployees() {
    $.get('/Employee/GetAll', function (res) {
        let rows = '';
        res.data.forEach(item => {
            rows += `<tr>
                <td>${item.empCode}</td>
                <td>${item.empName}</td>
                <td>${item.deptName}</td>  
                <td>${item.desigName}</td> 
                <td>${item.gross.toFixed(2)}</td>
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

// এডিট ফাংশন (টাইমিং ফিক্সড)
// এডিট ফাংশন (টাইমিং এবং রেস কন্ডিশন ফিক্সড, ড্রপডাউন লোডের পরে ড্রপডাউন ভ্যালু সেট করার জন্য ইভেন্ট হ্যান্ডলার যুক্ত করা হয়েছে)
function editEmp(id) {
    $.get('/Employee/GetAll', function (res) {
        const emp = res.data.find(e => e.empId === id);
        if (emp) {
            // ১. প্রথমে সাধারণ টেক্সট এবং নম্বর ফিল্ডগুলোর ভ্যালু সেট করুন
            $('#empId').val(emp.empId);
            $('#empCode').val(emp.empCode);
            $('#empName').val(emp.empName);
            $('#gross').val(emp.gross);
            
            if (emp.dtJoin) {
                $('#dtJoin').val(new Date(emp.dtJoin).toISOString().split('T')[0]);
            }

            // ২. এবার মোডালটি ওপেন করার নির্দেশ দিন
            $('#empModal').modal('show');

            // ৩. [ম্যাজিক পার্ট] মোডালটি স্ক্রিনে পুরোপুরি লোড হওয়া পর্যন্ত অপেক্ষা করুন, 
            // তারপর ড্রপডাউনের পূর্বের ভ্যালুগুলো সিলেক্ট করে দিন।
            $('#empModal').one('shown.bs.modal', function () {
                // .one() ব্যবহার করায় এই ইভেন্টটি শুধু এডিট বাটনে ক্লিক করলেই একবার ট্রিগার হবে
                $('#deptId').val(emp.deptId).change(); 
                $('#desigId').val(emp.desigId).change();
                if(emp.shiftId) {
                    $('#shiftId').val(emp.shiftId).change();
                }
            });
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

// এমপ্লয়ি সেভ করা
function saveEmployee() {
    const payload = {
        empId: $('#empId').val() || "00000000-0000-0000-0000-000000000000",
        empCode: $('#empCode').val(),
        empName: $('#empName').val(),
        deptId: $('#deptId').val(),
        desigId: $('#desigId').val(),
        shiftId: $('#shiftId').val() || null,
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