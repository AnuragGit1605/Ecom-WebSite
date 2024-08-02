var dataTable
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "isAuthorizedCompany", "width": "5%",
                "render": function (data) {
                    if (data) {
                        return `<input type="checkbox" checked disabled />`;
                    }
                    else {
                        return `<input type="checkbox" disabled />`;
                    }
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div class="text-center">
                       <a href="/Admin/Company/Upsert/${data}" class="btn btn-info">
                                <i class="fas fa-edit"></i>
                            </a>
                            <a class="btn btn-danger" onclick=Delete('/Admin/Company/Delete/${data}')>
                                <i class="fas fa-trash-alt"></i>
                            </a>
                    </div> `;
                }
            }

        ]
    })
}
function Delete(url) {
    swal({
        title: "Want to delete data?",
        text: "Delete Information",
        buttons: true,
        icon: "warning",
        dangerMode: true
    }).then((willDelete) => {
        console.log('deleting')
        if (willDelete) {
            $.ajax({
                url: url,
                data: { id: id },
                type: "DELETE",
                success: function (data) {

                    swal("Success", data.message, "success");

                    dataTable.ajax.reload();
                },
                error: function (xhr, status, error) {
                    toaster.error("Error occurred while deleting data.");
                }

            });
        }
    });
}