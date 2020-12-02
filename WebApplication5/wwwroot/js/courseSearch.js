$("#topicSelect").change((e) => {
    $(".course").show();
    $(".course").removeClass("searched");
    $(".course").each((i, el) => {
        if (el.querySelector(".keywords").value.includes(e.target.value)) {
            $(el).addClass("searched")
        }
    })
    $(".course").hide();
    $(".searched").show();
})

$("#sortSelect").change((e) => {
    key = encodeURIComponent("order");
    value = encodeURIComponent(e.target.value);

    var kvp = document.location.search.substr(1).split('&');
    let i = 0;

    for (; i < kvp.length; i++) {
        if (kvp[i].startsWith(key + '=')) {
            let pair = kvp[i].split('=');
            pair[1] = value;
            kvp[i] = pair.join('=');
            break;
        }
    }

    if (i >= kvp.length) {
        kvp[kvp.length] = [key, value].join('=');
    }

    let params = kvp.join('&');

    document.location.search = params;
})