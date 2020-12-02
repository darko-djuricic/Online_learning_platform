$(document).ready(() => {
    $(".deleteUser").click((e) => {
        let parent = $(e.target).parent().parent();
        $("#deleteModal").modal('show');
        $("#modalText").html(`Are you sure you want to delete user ${parent.find(".username").text()}?`)
        $("#deleteBtn").click(() => {
            parent.find(".deleteForm").submit();
            $("#deleteModal").modal('hide');
        });
        $("#deleteModal").modal('show');
    })

    $(".deleteCategory").click((e) => {
        let parent = $(e.target).parent().parent();
        $("#deleteModal").modal('show');
        $("#modalText").html(`Are you sure you want to delete comment "${parent.find(".title").text()}" ?`)
        $("#deleteBtn").click(() => {
            parent.find(".deleteForm").submit();
            $("#deleteModal").modal('hide');
        });
        $("#deleteModal").modal('show');
    })

    $(".selectRow").click((e) => {
        $("#editCategory").removeClass("disabled");
        let row = $(e.target).parent();
        if (!row.hasClass("selected")) $(".selectRow").removeClass("selected");
        row.toggleClass("selected");
        $("#categoryTitle").val(row.find(".title").text())
        let text = row.find(".parent").text().trim();

        if (text) {
            $("#parentCategory option").filter(function () {
                return $(this).text().trim() == text;
            }).prop('selected', true);
        }
        else
            $("#parentCategory").val(text);

        if ($(".selected").length == 0) $("#editCategory").addClass("disabled");
    });

    $("#addCategory").click((e) => {
        if ($("#categoryTitle").val().trim()) {
            $("#categoryForm").attr("action", `${location.origin}${location.pathname}/AddCategory`);
            $("#categoryForm").submit()
        }
    })

    $("#editCategory").click((e) => {
        if ($("#categoryTitle").val().trim() && $(".selected").length > 0) {
            $("#categoryForm").attr("action", `${location.origin}${location.pathname}/UpdateCategory/${$(".selected").find(".id").text()}`).submit();
        }
    })
})
