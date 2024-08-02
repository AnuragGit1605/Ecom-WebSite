var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll",
            
        },
        "columns": [
            { "data": "name", "width": "25%" }, 
            { "data": "orderDate", "width": "25%" }, 
            { "data": "orderTotal", "width": "25%" }, 
            {
                "data": "orderId",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Order/Detail/${data}" class="btn btn-info">
                                <i class="fas fa-edit"></i>
                                </a>
                            </div>`;
                }
            }
        ]
    });
}
