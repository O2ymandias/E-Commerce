$(document).ready(function () {

    $('.js-delete').on('click', function () {

        const btn = $(this);

        bootbox.confirm({
            message: 'Are you sure you want to delete this category?',
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
            callback: function (result) {
                if (result) {
                    $.ajax({
                        url: `/api/Categories/${btn.data('id')}`,
                        method: 'DELETE',
                        success: function () {
                            btn.parents('tr').fadeOut();
                            alert("Category has been deleted successfully.");
                        },
                        error: function () {
                            alert("Something went wrong.");
                        }
                    }
                    );
                }
            }
        });
    });
});
