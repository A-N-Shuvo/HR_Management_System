$(document).ready(function () {
    loadCompanies();
});

// সব কোম্পানির লিস্ট লোড করা
function loadCompanies() {
    $.ajax({
        url: '/Company/GetAll',
        type: 'GET',
        success: function (response) {
            let html = '';
            $.each(response.data, function (key, item) {
                html += `<tr>
                    <td>${item.comName}</td>
                    <td>${item.basic}</td>
                    <td>${item.hrent}</td>
                    <td>${item.medical}</td>
                    <td>${item.isInactive ? 'Inactive' : 'Active'}</td>
                    <td>
                        <button class="btn btn-sm btn-warning" onclick="editCompany('${item.comId}', '${item.comName}', ${item.basic}, ${item.hrent}, ${item.medical}, ${item.isInactive})">Edit</button>
                        <button class="btn btn-sm btn-danger" onclick="deleteCompany('${item.comId}')">Delete</button>
                    </td>
                </tr>`;
            });
            $('#companyTableBody').html(html);
        }
    });
}

// ডাটা সেভ বা আপডেট করা
function saveCompany() {
    const data = {
        comId: $('#comId').val() || "00000000-0000-0000-0000-000000000000",
        comName: $('#comName').val(),
        basic: parseFloat($('#basic').val()),
        hrent: parseFloat($('#hrent').val()),
        medical: parseFloat($('#medical').val()),
        isInactive: $('#isInactive').is(':checked')
    };

    $.ajax({
        url: '/Company/Upsert',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                alert(response.message);
                $('#companyModal').modal('hide');
                loadCompanies();
            } else {
                alert(response.message);
            }
        }
    });
}

// এডিট করার জন্য ডাটা মডালে সেট করা
function editCompany(id, name, basic, hrent, medical, inactive) {
    $('#modalTitle').text('Edit Company');
    $('#comId').val(id);
    $('#comName').val(name);
    $('#basic').val(basic);
    $('#hrent').val(hrent);
    $('#medical').val(medical);
    $('#isInactive').prop('checked', inactive === 'true');
    $('#companyModal').modal('show');
}

// ডিলিট অপারেশন
function deleteCompany(id) {
    if (confirm('Are you sure?')) {
        $.ajax({
            url: `/Company/Delete?id=${id}`,
            type: 'DELETE',
            success: function (response) {
                alert(response.message);
                loadCompanies();
            }
        });
    }
}

// ফর্ম রিসেট করা
function resetForm() {
    $('#comId').val('');
    $('#companyForm')[0].reset();
    $('#modalTitle').text('Add Company');
}