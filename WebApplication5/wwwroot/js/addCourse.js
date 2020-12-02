function selectElementContents(el) {
    var range = document.createRange();
    range.selectNodeContents(el);
    var sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);
}

function addBadge(skip = false, data="Keyword") {
    let keywords = [];
    let inputs = document.querySelectorAll('.keyword');
    inputs.forEach(el => {
        keywords.push(el.textContent.trim())
    })
    let inserted = inputs[inputs.length - 1];
    if (inserted && (inserted.textContent.trim() == data) || keywords.length != [...new Set(keywords)].length) {
        inserted.remove();
    }
    $("#start").remove();
    $("#zoom").before(
        `<span class="badge mt-2  pt-2 pb-2 badge-pill badge-secondary mr-2" ${skip ? "hidden" : ""}><span class="keyword" contenteditable="true">${data}</span><span contenteditable="false" onclick="$(this).parent().remove()" class="closeIcon ml-2 h5" readonly">&times;</span></span>`
    )
    inputs = document.querySelectorAll('.keyword');
    inserted = inputs[inputs.length - 1];
    $(inserted).on('keydown paste', function (event) {
        if ($(this).text().length > 20 && event.keyCode != 8) {
            event.preventDefault();
        }
    })
    keywords = [];

    inputs.forEach(el => {
        if (el.textContent != "Keyword") {
            keywords.push(el.textContent.trim())
        }

    })
    
    selectElementContents(inserted)
    inserted.focus();
    $("#keywords").val([...new Set(keywords)].join(";"))

    $(".keyword").keyup(function (e) {
        var evtobj = window.event ? event : e;
        if ((evtobj.keyCode == 83 && evtobj.ctrlKey) || evtobj.key == "Control" || evtobj.keyCode == 116) {
            return false;
        }
        changed = true;
    })
}


$("#zoom").click(() => {
    addBadge();
});
$("#btnSubmit").click(() => {
    addBadge(true);
    $("#courseForm").submit();
})
