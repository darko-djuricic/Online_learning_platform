let courses = [];
let filterCourses = [];

$(document).ready(() => {
    $(".spinner").hide();
    let xhr = $.ajax({
        type: "GET",
        url: path + `/CoursesByTeacher/${username}`,
        success: (data) => {
            courses = data;
        }
    }).done(() => {
        $(".checkCourse").change(loadComments)
        $("input[name=radioComments]").change(loadComments)
        $("#postReply").click(update);
        $("#deleteBtn").click(update);
    });

    function loadComments() {
        xhr.abort();
        $(".response").remove();
        filterCourses = courses.filter(el => Array.from($(".checkCourse").filter((i, el) => el.checked).map((i, el) => el.value)).some(el2 => el2 == el.courseId));
        filterCourses.forEach((element) => {
            $(".spinner").show();
            $(".checkCourse").attr("disabled","");
            if ($('input[name=radioComments]:checked').val() == 0) {
                xhr=$.ajax({
                    type: "GET",
                    url: `${window.location.protocol}//${window.location.host}/Course/Info/${element.courseId}`,
                    success: function (response) {
                        let text = $(response).find("#commentsection").html().trim();
                        $("#loader").append(text?`<ul class='list-unstyled response shadow-sm'>
                                                <span class='bg-primary text-light mb-2 d-block pt-2 pb-2 pl-2 h5 font-weight-bold'>"${element.title}"</span>
                                                <div class="pl-3 pr-3">${text}
                                                </div>
                                            </ul>`: "");
                        Start();
                    }
                }).done(() => {
                    $(".spinner").hide();
                    $(".checkCourse").removeAttr("disabled");
                });
            }
            else {
                xhr = $.ajax({
                    type: "GET",
                    url: `${window.location.protocol}//${window.location.host}/Course/Lecture/${element.courseId}`,
                    success: function (response) {
                        let text = $(response).find("#qaSection").html();
                        if (text)
                            $("#loader").append(`<ul class='list-unstyled response shadow-sm'><span class='bg-primary text-light mb-2 d-block pt-2 pb-2 pl-2 h5 font-weight-bold'>"${element.title}"</span><div class="pl-3 pr-3">` + text + "</div></ul>");
                        Start();
                    }
                }).done(() => {
                    $(".spinner").hide();
                    $(".checkCourse").removeAttr("disabled");
                });
            }
        });
    }

    function update() {
        $(".response").remove();
        $(".spinner").show();
        setTimeout(loadComments, 1000);
    }

    $("#updateBtn").click(update);
})