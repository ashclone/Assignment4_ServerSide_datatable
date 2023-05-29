var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/ADOEmployee/GetAll"

        },
        "columns": [
            { "data": "firstName" },
            { "data": "lastName" },
            {
                data: "gender", render: function (data) {
                    if (data == 0)
                        return "Male";
                    else if (data == 1)
                        return "Female";
                    else
                        return "Other";

                }
            },

            {
                "data": "birthDate", 'render': function (data, type, full, meta) {
                    if (type === 'display') {
                        data = data.substr(0, 10);
                    }
                    return data;
                }
            },
            { "data": "address" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                            <a href="/adoemployee/edit/${data}" class="btn btn-success m-2">
                            <i class="fas fa-edit"></i> </a>                           
                            <a class="btn btn-danger" onclick=Delete("/adoemployee/Delete/${data}")>
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
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        Swal.fire({
                            title: 'Deleted!',
                            text: 'data Deleted Successfully ',
                            icon: 'success'
                        }
                        )
                        datatable.ajax.reload();
                    }
                    else {

                        Swal.fire({
                            title: 'Cancelled',
                            text: "Error while Deleting data in DataBase",
                            icon: 'error'
                        }
                        )
                    }
                }
            })

        }
    })
}