$(document).ready(function() {

    $('.js-delete').on('click', function() {

        const btn = $(this);

        bootbox.confirm({
            message: 'Are you sure you want to delete this role?',
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-danger'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-secondary'
                }
            },
            callback: function(result) {
                if (result) {
                    $.ajax({
                        url: `/api/Roles/${btn.data('id')}`,
                        method: 'DELETE',
                        success: function() {
                            btn.parents('tr').fadeOut();
                            alert("Role has been deleted successfully.");
                        },
                        error: function() {
                            alert("Something went wrong.");
                        }
                    }
                    );
                }
            }
        });
    });
});
