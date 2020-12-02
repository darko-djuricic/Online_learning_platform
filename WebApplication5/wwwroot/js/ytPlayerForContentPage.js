let element = $(".activeVideo").find(".listened")[0];
const listened = element ? element.checked : null;

var tag = document.createElement('script');
tag.id = 'iframe-demo';
tag.src = 'https://www.youtube.com/iframe_api';
var firstScriptTag = document.getElementsByTagName('script')[0];
firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

let timer;
let seconds = 0;

var players = [];
function onYouTubeIframeAPIReady() {
    for (var i = 0; i < $(".ytVideo").length; i++) {
        players[i] = new YT.Player('existing-iframe-example' + i, {
            events: {
                'onReady': onPlayerReady,
                'onStateChange': onPlayerStateChange
            }
        });
    }
}

function onPlayerReady(event) {
    $('.ytVideo').css("border-color", '#FF6D00');
}

function onPlayerStateChange(event) {
    changeBorderColor(event.data, event.target);
    if (event.target.getDuration() > 3600) {
        event.target.loadVideoById("emptylink", 0);
        alert("Video is longer then one our!")
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

