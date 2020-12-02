$(".deleteCourse").click((e) => {
    $("#modalText").html(`Are you sure you want to delete this course?`)
    $("#deleteBtn").click(() => {
        $("#deleteForm").submit();
        $("#deleteModal").modal('hide');
    });
    $("#deleteModal").modal('show');
});