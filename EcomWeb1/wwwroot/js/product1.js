var datatable
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "20%" },
            { "data": "description", "width": "25%" },
            { "data": "isbn", "width": "20%" },
            { "data": "price", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
<div class="text-center">
<a href="/Admin/Product/Upsert/${data}" class="btn btn-info">
<i class="fas fa-edit"></i>
</a>
<a class="btn btn-danger" onclick=Delete('/Admin/Product/Delete/${data}')>
<i class="fas fa-trash-alt"></i>
</a>
</div>
`;
                }
            }
        ]
    })
}
function Delete(url) {
     alert(url);
    swal({
        title: "Want to Delete Data ?",
        text: "Data cannot Recover once deleted !!!",
        icon: "warning",
        buttons: true,
        dangerModel: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}