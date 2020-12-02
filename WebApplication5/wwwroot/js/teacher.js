$(document).ready(() => {
    $("#shadowDiv").hover(() => {
        $("#shadowDiv").toggleClass("shadow-lg");
    })
    $(".searchCourses").keyup((e) => {
        let courses = document.querySelectorAll(".course");
        $('.course').removeClass("searched");
        $(".course").hide();
        courses.forEach((el, i) => {
            let target = e.target.value.trim().toLowerCase();
            let title = el.querySelector(".courseTitle").textContent.trim().toLowerCase();
            let desc = el.querySelector(".courseDesc").textContent.trim().toLowerCase();
            let condition = title.search(target) > -1 || desc.search(target) > -1
            $(el).addClass(condition ? "searched" : "");
        });

        //$("#status").text($(".searched").length > 0 ? "" : "No results.");
        $(".searched").show();
    })

    $('input[name="courseRadios"]').change((e) => {
        switch (parseInt($('input[name="courseRadios"]:checked').val())) {
            case 0:
                $(".course").show();
                return;
            case 1:
                $(".course").hide();
                $(".published").show();
                return;
            case -1:
                $(".course").show();
                $(".published").hide();
                return;
            default:
                return false;
        }
    });
});