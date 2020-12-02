let element = $(".activeVideo").find(".listened")[0];
const listened = element ? element.checked : null;

var tag = document.createElement('script');
tag.id = 'iframe-demo';
tag.src = 'https://www.youtube.com/iframe_api';
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

let timer;
let seconds = 0;

var player;
function onYouTubeIframeAPIReady() {
    player = new YT.Player('existing-iframe-example', {
        events: {
            'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
        }
    });
}
function onPlayerReady(event) {
    $('.ytVideo').css("border-color", '#FF6D00');
}

$(".sectionLecture").each((i, el) => {
    let condition = Array.from($(el).find(".contentPart").find(".listened")).every(el => el.checked);
    if (condition) {
        $(el).find(".sectionListened")[0].checked = true;
    }
})

function IsListened() {
    clearInterval(timer);
    if (listened != null) {
        let path = `${window.location.protocol}//${window.location.host}/api/Values`;
        let pathname = window.location.pathname.split("/");
        let id = pathname[pathname.length - 1];
        let duration = Math.round(player.getDuration());
        if (seconds >= (duration/2)) {
            $.ajax({
                type: "GET",
                url: path + `/Listened/${$("#idContent").val()}/${id}`,
            });
        }
    }
}

function onPlayerStateChange(event) {
    changeBorderColor(event.data, event.target);
    if (event.data == YT.PlayerState.PLAYING) {
        timer = setInterval(
            function () {
                seconds++;
            }, 1000
        );
    }
    else {
        clearInterval(timer);
    }
    if (event.data == 0) {
        IsListened();

        let link = $("#linkToNextVideo").attr("href");
        if (link != null) {
            const urlParams = new URLSearchParams(window.location.search);
            setTimeout(() => {
                window.location = link;
            }, 5000)
        }
    }
}

function changeBorderColor(playerStatus, el) {
    var color;
    if (playerStatus == -1) {
        color = "#37474F"; // unstarted = gray
    } else if (playerStatus == 0) {
        color = "#FFFF00"; // ended = yellow
    } else if (playerStatus == 1) {
        color = "#33691E"; // playing = green
    } else if (playerStatus == 2) {
        color = "#DD2C00"; // paused = red
    } else if (playerStatus == 3) {
        color = "#AA00FF"; // buffering = purple
    } else if (playerStatus == 5) {
        color = "#FF6DOO"; // video cued = orange
    }
    if (color) {
        el.f.style.borderColor = color;
    }
}


window.onunload = IsListened;

