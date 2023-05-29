var datatable;
$(document).ready(function () {
    datatable = $('#myTable').DataTable(
        {
            ajax: {
                url: "employees/getall",
                type: "POST",
            },
            processing: true,
            serverSide: true,
            filter: true,
            "search": true,
            sorting: true,
            order: [[0, 'asc']],


            columns: [
                { data: "firstName", name: "FirstName"},
                { data: "lastName", name: "LastName" },
                {
                    data: "gender", render: function (data) {
                        if (data == 0)
                            return "Male";
                        else if (data == 1)
                            return "Female";
                        else
                            return "Other";

                    }, name: "Gender", sorting: false, orderable: false
                },

                { data: "address", name: "Address" },
                {
                    data: "id",
                    "render": function (data) {
                        return `
                            <div class="text-center">
                            <a href="/employees/edit/${data}" class="btn btn-success m-2">
                            <i class="fas fa-edit"></i> </a>                           
                            <a class="btn btn-danger" onclick=Delete("/employees/Delete/${data}")>
                            <i class="fas fa-trash-alt"></i>
                            </a>
                        </div>

                    `;

                    }, name: "Id",
                    orderable:false

                }
            ]
        }
    );
});

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

