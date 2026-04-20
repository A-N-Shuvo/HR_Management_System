$(document).ready(function () {
    const now = new Date();
    $('#dtYear').val(now.getFullYear());
    $('#dtMonth').val(now.getMonth() + 1);
});

function loadSalary() {
    const year = $('#dtYear').val();
    const month = $('#dtMonth').val();

    $.get(`/Salary/GetSalaryList?year=${year}&month=${month}`, function (res) {
        let rows = '';
        if (res.data.length > 0) {
            res.data.forEach(item => {
                rows += `<tr>
                    <td>${item.empCode}</td>
                    <td>${item.empName}</td>
                    <td class="text-end">${item.gross.toFixed(2)}</td>
                    <td class="text-end text-danger">${item.absentAmount.toFixed(2)}</td>
                    <td class="text-end fw-bold">${item.payableAmount.toFixed(2)}</td>
                    <td class="text-center">
                        ${item.isPaid
                        ? '<span class="badge bg-success">Paid</span>'
                        : `<button class="btn btn-sm btn-primary" onclick="paySalary('${item.id}')">Pay Now</button>`}
                    </td>
                </tr>`;
            });
        } else {
            rows = '<tr><td colspan="6" class="text-center">No salary data found. Please calculate.</td></tr>';
        }
        $('#salaryTableBody').html(rows);
    });
}

function processSalary() {
    const year = $('#dtYear').val();
    const month = $('#dtMonth').val();

    $.post(`/Salary/CalculateSalary?year=${year}&month=${month}`, function (res) {
        if (res.success) {
            alert(res.message);
            loadSalary();
        } else {
            alert("Error: " + res.message);
        }
    });
}

function paySalary(id) {
    if (confirm("Are you sure you want to mark this as Paid?")) {
        $.post(`/Salary/UpdatePaymentStatus/${id}`, function (res) {
            if (res.success) {
                alert(res.message);
                loadSalary();
            }
        });
    }
}