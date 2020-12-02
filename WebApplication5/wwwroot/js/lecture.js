var taLineHeight = 15;
var taHeight = document.querySelector("#lectureText").scrollHeight;
var numberOfLines = Math.floor(taHeight / taLineHeight);
$("#lectureText").attr("rows", `${numberOfLines}`);
$("#startNav").remove();


