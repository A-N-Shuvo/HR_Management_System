$(document).ready(function () {
    loadShifts();
});

function loadShifts() {
    $.get('/Shift/GetAll', function (res) {
        let rows = '';
        res.data.forEach(item => {
            rows += `<tr>
                <td>${item.shiftName}</td>
                <td>${item.inTime}</td>
                <td>${item.outTime}</td>
                <td>${item.lateTime}</td>
                <td>
                    <button class="btn btn-sm btn-warning" onclick="editShift('${item.shifid}', '${item.shiftName}', '${item.inTime}', '${item.outTime}', '${item.lateTime}')">Edit</button>
                    <button class="btn btn-sm btn-danger" onclick="deleteShift('${item.shifid}')">Delete</button>
                </td>
            </tr>`;
        });
        $('#shiftTableBody').html(rows);
    });
}

// নতুন ডিলিট ফাংশন
function deleteShift(id) {
    if (confirm("Are you sure you want to delete this shift?")) {
        $.ajax({
            url: '/Shift/Delete/' + id,
            type: 'DELETE',
            success: function (res) {
                if (res.success) {
                    alert(res.message);
                    loadShifts();
                } else {
                    alert(res.message); // এখানে এমপ্লয়ি অ্যাসাইন থাকলে এরর দেখাবে
                }
            },
            error: function () {
                alert("Something went wrong!");
            }
        });
    }
}


function editShift(id, name, inTime, outTime, lateTime) {
    $('#shifid').val(id);
    $('#shiftName').val(name);

    // TimeSpan (09:00:00) থেকে input[type=time] এর জন্য (09:00) ফরম্যাট করা
    $('#inTime').val(inTime.substring(0, 5));
    $('#outTime').val(outTime.substring(0, 5));
    $('#lateTime').val(lateTime.substring(0, 5));

    $('#shiftModal').modal('show');
}

function saveShift() {
    const data = {
        shifid: $('#shifid').val() || "00000000-0000-0000-0000-000000000000",
        shiftName: $('#shiftName').val(),
        // TimeSpan ফরম্যাট নিশ্চিত করতে সেকেন্ড সহ পাঠানো ভালো (HH:mm:ss)
        inTime: $('#inTime').val() + ":00",
        outTime: $('#outTime').val() + ":00",
        lateTime: $('#lateTime').val() + ":00"
    };

    $.ajax({
        url: '/Shift/Upsert',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (res) {
            if (res.success) {
                $('#shiftModal').modal('hide');
                loadShifts();
                alert(res.message);
            }
        }
    });
}

function resetShiftForm() {
    $('#shifid').val('');
    $('#shiftForm')[0].reset();
}