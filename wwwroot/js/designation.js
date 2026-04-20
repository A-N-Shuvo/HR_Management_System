$(document).ready(function () {
    loadDesignations();
});

function loadDesignations() {
    $.ajax({
        url: '/Designation/GetAll',
        type: 'GET',
        success: function (res) {
            let rows = '';
            $.each(res.data, function (i, item) {
                rows += `<tr>
                    <td>${item.desigName}</td>
                    <td>
                        <button class="btn btn-sm btn-warning" onclick="edit('${item.desigId}', '${item.desigName}')">Edit</button>
                        <button class="btn btn-sm btn-danger" onclick="remove('${item.desigId}')">Delete</button>
                    </td>
                </tr>`;
            });
            $('#desigTableBody').html(rows);
        }
    });
}

function saveDesignation() {
    const data = {
        desigId: $('#desigId').val() || "00000000-0000-0000-0000-000000000000",
        desigName: $('#desigName').val()
    };

    $.ajax({
        url: '/Designation/Upsert',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (res) {
            if (res.success) {
                $('#desigModal').modal('hide');
                loadDesignations();
                alert(res.message);
            }
        }
    });
}

function edit(id, name) {
    $('#modalTitle').text('Edit Designation');
    $('#desigId').val(id);
    $('#desigName').val(name);
    $('#desigModal').modal('show');
}

function remove(id) {
    if (confirm('Delete this designation?')) {
        $.ajax({
            url: `/Designation/Delete?id=${id}`,
            type: 'DELETE',
            success: function (res) {
                loadDesignations();
            }
        });
    }
}

function resetForm() {
    $('#desigId').val('');
    $('#desigForm')[0].reset();
}