let path = `${window.location.protocol}//${window.location.host}/api/Values`;
$(".spinnerComment").hide();
$('.modal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget)
    var recipient = button.data('comment')
    var id = button.data('id');
    var modal = $(this)
    modal.find('.comment-for-reply').val(recipient)
    modal.find('.comment-for-reply').attr("id", id);
})

let users = [...new Set(Array.from($(".commentAuthor")).map(el => el.textContent))];
let userColor = {};
users.forEach(el => {
    userColor[el] = `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`;
})

function UserImg() {
    $(".userImg").each((i, el) => {
        let user = $(el).parent().find(".commentAuthor").first().text();
        let color = userColor[user];
        if (color) {
            $(el).css("background-color", color)
        }
        else {
            userColor[user] = `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`;
            $(el).css("background-color", userColor[user])
        }
        $(el).addClass("checked");
    })
}

function Start() {
    UserImg();

    $(".searchComments").keyup((e) => {
        $('.question').show();
        $("#qaSection li").show();
        let titles = document.querySelectorAll(".commentTitle");
        $(".question").removeClass("searched")
        titles.forEach((el, i) => {
            let target = e.target.value.trim().toLowerCase();
            let text = el.textContent.trim().toLowerCase();
            let body = el.parentElement.querySelector(".commentBody")
            let bodyText = body == null ? "" : body.textContent.trim().toLowerCase();
            let condition = text.search(target) > -1 || bodyText.search(target) > -1
            $(el).parent().parent().addClass(condition ? "searched" : "");
        });

        $("#status").text($(".searched").length > 0 ? "" : "No results.");

        $(".question").hide();
        $(".searched").show();
    })

    $("#qaSelection").change((e) => {
        if (e.target.value == "all") {
            $("#qaSection li").show();
            $("#numberOfQA").text($(".question").length)
        }
        else {
            $("#qaSection li").hide();
            $(".thisLecture").show();
            $("#numberOfQA").text($(".thisLecture").length)
        }
    })

    $(".deleteComment").click((e) => {
        $("#deleteModal").modal('show');
        $("#modalText").html(`Are you sure you want to delete your comment?`)
        $("#deleteBtn").click(() => {
            $.ajax({
                type: "GET",
                url: path + "/DeleteComment/" + e.target.id,
                success: function (response) {
                    $("#load").load(`${window.location.href} #commentsOrQa`, () => {
                        Start();
                        $("#qaSelection").trigger('change');
                    });
                }
            });
            $("#deleteModal").modal('hide');
        });
        $("#deleteModal").modal('show');
    })

    $(".likeButton").each((i, el) => {
        $(el).change((e) => {
            let myState;
            let numOfLikesEl = e.target.parentElement.querySelector(".likes");
            let num = parseInt(numOfLikesEl.textContent);
            e.target.checked ? num++ : num--;
            numOfLikesEl.textContent = num;

            clearTimeout(myState);
            myState = setTimeout(() => {
                let path = `${window.location.protocol}//${window.location.host}/api/Values/LikeUnlike`;
                if (myState != null) {
                    $.ajax({
                        type: "GET",
                        url: path + `/${e.target.value}?IsLiked=${e.target.checked}`
                    });
                }
                
            }, 500);
        });
    });

    $("#qaSection li").hide();
    $(".thisLecture").show();
}

$("#commentsOrQa").scroll(() => {
    $("[data-toggle='popover']").popover('hide');
})

$("#postQa").click(() => {
    $(".spinnerComment").show();
    let path = `${window.location.protocol}//${window.location.host}/api/Values/AddQA`;
    $.ajax({
        type: "GET",
        url: path + `/${$("#idContent").val()}`,
        traditional: true,
        data: $("#qaForm").serialize() + "&author=user",
        success: (data) => {
            $('.modal').modal('hide');
            $("#load").load(`${window.location.href} #commentsOrQa`, () => {
                Start();
                $("#qaSelection").trigger('change');
            });
        }
    }).done(() => { $(".spinnerComment").hide(); });
})

$("#postComment").click(() => {
    $(".spinnerComment").show();
    let rating = Array.from($(".ratingStar")).map(el => el.checked).reverse().indexOf(true) + 1;
    let pathname = window.location.pathname.split("/");
    let id = pathname[pathname.length - 1];
    $.ajax({
        type: "GET",
        url: path + `/Rating/${id}`,
        traditional: true,
        data: `rating=${rating}`,
        success: function (data) {
            $.ajax({
                type: "GET",
                url: path + `/AddComment/${id}`,
                traditional: true,
                data: $("#commentForm").serialize() + "&author=user",
                success: (data) => {
                    $('#commentModal').modal('hide');
                    $("#load").load(`${window.location.href} #load`, () => {
                        Start();
                    });
                }
            });
        }
    }).done(() => {
        $(".spinnerComment").hide();
        $("#addReview").html('<p class="float-right text-success">Thanks for review!</p>');
    });
    
})

$("#postReply").click(() => {
    $(".spinnerComment").show();
    let path = `${window.location.protocol}//${window.location.host}/api/Values/AddReply`;
    $.ajax({
        type: "GET",
        url: path + `/${$(".comment-for-reply").attr("id")}`,
        traditional: true,
        data: $("#replyForm").serialize(),
        success: (data) => {
            $(".modal").modal('hide');
            $("#load").load(`${window.location.href} #commentsOrQa`, () => {
                Start();
                $("#qaSelection").trigger('change');
            });
        },
    }).done(() => { $(".spinnerComment").hide();});
})

Start();
