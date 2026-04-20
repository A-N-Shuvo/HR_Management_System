$(document).ready(function () {
    loadDepartments();
});

function loadDepartments() {
    $.ajax({
        url: '/Department/GetAll',
        type: 'GET',
        dataType: 'json',
        success: function (res) {
            let html = '';
            if (res.data.length === 0) {
                html = '<tr><td colspan="2" class="text-center text-muted">No departments found for this company.</td></tr>';
            } else {
                $.each(res.data, function (i, item) {
                    html += `<tr>
                        <td class="align-middle">${item.deptName}</td>
                        <td class="text-center">
                            <button class="btn btn-sm btn-info text-white" onclick="editDept('${item.deptId}', '${item.deptName}')">
                                <i class="bi bi-pencil-square"></i> Edit
                            </button>
                            <button class="btn btn-sm btn-danger" onclick="deleteDept('${item.deptId}')">
                                <i class="bi bi-trash"></i> Delete
                            </button>
                        </td>
                    </tr>`;
                });
            }
            $('#deptTableBody').html(html);
        },
        error: function () {
            alert("Error loading data!");
        }
    });
}

function saveDepartment() {
    const deptName = $('#deptName').val();
    if (!deptName) {
        alert("Please enter department name");
        return;
    }

    const payload = {
        deptId: $('#deptId').val() || "00000000-0000-0000-0000-000000000000",
        deptName: deptName
    };

    $.ajax({
        url: '/Department/Upsert',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (res) {
            if (res.success) {
                $('#deptModal').modal('hide');
                loadDepartments();
                resetDeptForm();
            } else {
                alert(res.message);
            }
        }
    });
}

function editDept(id, name) {
    $('#deptModalTitle').text('Update Department');
    $('#deptId').val(id);
    $('#deptName').val(name);
    $('#deptModal').modal('show');
}

function deleteDept(id) {
    if (confirm('Are you sure you want to delete this department?')) {
        $.ajax({
            url: `/Department/Delete?id=${id}`, // মেথড নাম 'Delete' এবং প্যারামিটার 'id'
            type: 'DELETE', // কন্ট্রোলারের HttpDelete এর সাথে মিল রেখে
            success: function (res) {
                if (res.success) {
                    loadDepartments();
                    alert(res.message);
                } else {
                    alert(res.message);
                }
            }
        });
    }
}

function resetDeptForm() {
    $('#deptId').val('');
    $('#deptName').val('');
    $('#deptModalTitle').text('Add New Department');
}