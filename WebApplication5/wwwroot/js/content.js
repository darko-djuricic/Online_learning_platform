let path = `${window.location.protocol}//${window.location.host}/api/Values`;
let youtubePath = "https://www.youtube.com/embed/";
let changed = false;

function status(response = "Alert", error = false) {
    $("#status").html(`
        <div class="alert alert-${error?'warning':'success'} sticky-top alert-dismissible fade show" role="alert">
            ${response}
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span class="text-white" aria-hidden="true">&times;</span>
            </button>
        </div>
    `);
    setTimeout(function () {
        $(".alert").fadeTo(500, 0).slideUp(500, function () {
            $(this).remove();
        });
    }, 3000);
}

window.addEventListener("beforeunload", function (e) {
    if (changed) {
        var confirmationMessage = 'It looks like you have been editing something. '
            + 'If you leave before saving, your changes will be lost.';

        e.returnValue = confirmationMessage;
        return confirmationMessage; 
    }
});

$(window).bind("change", function (e) {
    changed = true;
})

$(".form-control").bind("keyup", function (e) {
    var evtobj = window.event ? event : e;
    if ((evtobj.keyCode == 83 && evtobj.ctrlKey) || evtobj.key == "Control" || evtobj.keyCode == 116) {
        return false;
    }
    changed = true;
})

$("span[contenteditable='true']").bind("keyup", function (e) {
    var evtobj = window.event ? event : e;
    if ((evtobj.keyCode == 83 && evtobj.ctrlKey) || evtobj.key == "Control" || evtobj.keyCode == 116) {
        return false;
    }
    changed = true;
})

function SaveChanges(e) {
    var evtobj = window.event ? event : e
    if (evtobj.keyCode == 83 && evtobj.ctrlKey) {
        e.preventDefault();
        if (changed)
            submitCourse();
        else {
            status("Nothing to save", true);
        }
    }
}

function IDOfYtVideo(link) {
    var video_id = link.split('v=')[1];
    if (!video_id) return "emptylink";
    var ampersandPosition = video_id.indexOf('&');
    if (ampersandPosition != -1) {
        video_id = video_id.substring(0, ampersandPosition);
    }
    return video_id;
}

function testVideo(el = $())  {
    let player = players[parseInt(el.find(".ytVideo").attr("data-number"))];
    player.loadVideoById(IDOfYtVideo(el.find('.videoInput').val()), 0);
}

function getDuration(player) { return player.getDuration() };

function addContent(element = $()) {
    let numberOfContent = parseInt(element.find($(".numberOfContent")).last().clone().text());
    numberOfContent = isNaN(numberOfContent) ? 1 : numberOfContent + 1;
    if (numberOfContent <= 10) {
        let numberOfSection = parseInt(element.parent().find($(".numberOfSection")).first().clone().text());
        let numberPlayer = $(".ytVideo").length;
        let a = element.find(".addContent").parent().before(`
        <div class="row dashed ml-2 mr-2 pl-0 pl-lg-4">
            <div class="content pt-4 pb-1 pl-3 pr-5" style="width: 94%">
                <div class="position-absolute">
                    <span contenteditable="false" class="h5 p-2 font-weight-bold"><span class="numberOfContent">${numberOfContent}</span>. </span>
                    <span contenteditable="true" class="shadow p-2 pr-5 h5 contentTitle font-weight-bold" type="text">Content title ${numberOfContent}</span>
                </div>
                <input class="idContent" type="text" value="" hidden />
                <a class="contentCollapseLink d-block text-right text-primary h3 " href="#content${numberOfSection}${numberOfContent}" data-toggle="collapse">
                    <span class="dropdown-toggle"></span>
                </a>
                <div class="contentCollapse collapse pl-3" id="content${numberOfSection}${numberOfContent}")>
                    <div class="row mt-4">
                        <div class="col-xl-6">
                            <div class="row">
                                <div class="form-group pr-0 pr-lg-3 mb-3 col-12">
                                    <label class="videoLabel h6">Video</label>
                                    <div class="input-group">
                                        <input value="" name="VideoLink" type="text" class="form-control videoInput" placeholder="Copy Youtube Video Link Here">
                                        <div class="input-group-append">
                                            <button class="btnTestVideo btn btn-outline-primary input-group-text">Test</button>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-12 mb-5 mb-lg-3">
                                    <h6>Test video...</h6>
                                    <div class="embed-responsive embed-responsive-21by9">
                                        <iframe id="existing-iframe-example${numberPlayer}"
                                                data-number="${numberPlayer}"
                                                class="embed-responsive-item ytVideo"
                                                src="https://www.youtube.com/embed/emptylink)?enablejsapi=1"
                                                frameborder="0" allowfullscreen
                                                style="border: solid 2px #37474F">
                                        </iframe>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xl-6">
                            <div class="form-group">
                                <label for="textContent" class="h6">Text Content</label>
                                <textarea class="form-control textContents" rows="17"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <span class="deleteContent h3 zoom text-danger text-right float-right pl-0 pl-md-1 pt-4 pb-2">&times;</span>
        </div>
        `);
        let content = $(element).find(".content").last();
        content.find("[data-toggle='collapse']").click((e) => {
            $(e.target).find(".dropdown-toggle").toggleClass("rotate");
        })
        content.find(".btnTestVideo").click((e) => {
            testVideo($(e.target).parent().parent().parent().parent())
        });

        content.parent().find(".deleteContent").click((e) => {
            let content = e.target.parentElement;
            $("#modalText").html(`Are you sure you want to delete content ${content.querySelector('.numberOfContent').textContent}. - "${content.querySelector('.contentTitle').textContent}"?`)
            $("#deleteBtn").click(() => {
                removeContent($(content))
                $("#deleteModal").modal('hide');
            });
            $("#deleteModal").modal('show');
        });
        players[numberPlayer] = new YT.Player('existing-iframe-example' + numberPlayer, {
                                        events: {
                                            'onReady': onPlayerReady,
                                            'onStateChange': onPlayerStateChange
                                        }
        });
        changed = true;
    }
    
}

function addSection() {
    let number = $(".numberOfSection").length + 1;

    if (number <= 10) {
        $("#plusSection").before(`
<div class="row pl-0 pl-lg-5">
    <div class="section bg-primary text-light p-2 pt-0 border" style="width: 95%">
        <div class="position-absolute">
            <div class="row ml-3 mt-2">
                <span contenteditable="false" class="h4 d-inline-block pt-2 font-weight-bold mr-3">Section <span contenteditable="false" class="numberOfSection d-inline-block">${number}</span>: </span>
                <span contenteditable="true" class="border border-white p-2 d-inline-block h4 sectionTitle font-weight-bold" type="text">Section title ${number}</span>
            </div>
        </div>
        <a class="sectionCollapseLink d-block pt-2 pb-2 text-decoration-none text-right text-light mr-2 h3 " data-toggle="collapse" href="#section${number}">
            <span class="dropdown-toggle"></span>
        </a>
        <input class="idSection" type="text" value="" hidden />

        <div class="collapse sectionContents bg-white text-primary" id="section${number}">
            <div class="row dashed ml-2 mr-2 pl-0 pl-lg-4">
                <div class="content pt-4 pb-1 pl-3 pr-5" style="width: 94%">
                    <div class="position-absolute">
                        <span contenteditable="false" class="h5 p-2 font-weight-bold"><span class="numberOfContent">1</span>. </span>
                        <span contenteditable="true" class="shadow p-2 pr-5 h5 contentTitle font-weight-bold" type="text">Content title 1</span>
                    </div>
                    <input class="idContent" type="text" value="" hidden />
                    <a class="contentCollapseLink d-block text-right text-primary h3 " href="#content${number}1" data-toggle="collapse">
                        <span class="dropdown-toggle"></span>
                    </a>
                    <div class="contentCollapse collapse pl-3" id="content${number}1">
                        <div class="row mt-4">
                            <div class="col-xl-6">
                                <div class="row">
                                    <div class="form-group pr-0 pr-lg-3 mb-3 col-12">
                                        <label class="videoLabel h6">Video</label>
                                        <div class="input-group">
                                            <input value="" name="VideoLink" type="text" class="form-control videoInput" placeholder="Copy Youtube Video Link Here">
                                            <div class="input-group-append">
                                                <button class="btnTestVideo btn btn-outline-primary input-group-text">Test</button>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-12 mb-5 mb-lg-3">
                                        <h6>Test video...</h6>
                                        <div class="embed-responsive embed-responsive-21by9">
                                                    <iframe id="existing-iframe-example${$("ytVideo").length}"
                                                            data-number="${$("ytVideo").length}"
                                                            class="embed-responsive-item ytVideo"
                                                            src="https://www.youtube.com/embed/emptylink?enablejsapi=1"
                                                            frameborder="0" allowfullscreen
                                                            style="border: solid 2px #37474F">
                                                    </iframe>
                                                </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xl-6">
                                <div class="form-group">
                                    <label for="textContent" class="h6">Text Content</label>
                                    <textarea class="form-control textContents" rows="17"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <span class="deleteContent position-static h3 zoom text-danger float-right pl-0 pl-md-1 pt-4 pb-2">&times;</span>
            </div>
            <div class="plusContent container d-flex justify-content-center mt-3">
                <img class="addContent zoom mb-3" src="/img/plus2.svg" width="50" alt="Add content" />
            </div>
        </div>
    </div>
<span class="deleteSection h3 position-static zoom pl-0 pl-md-1 pt-3 pb-2">&times;</span>
</div>`);
        let section = $(".section").last();
        section.find($(".plusContent")).click((e) => { addContent($(e.target).parent().parent()) });
        section.find("[data-toggle='collapse']").click((e) => {
            $(e.target).find(".dropdown-toggle").toggleClass("rotate");
        })
        section.find(".btnTestVideo").click((e) => {
            testVideo($(e.target).parent().parent().parent().parent());
        });

        $(".deleteSection").last().click((e) => {
            let section = e.target.parentElement;
            $("#deleteSectionTitle").text(section.querySelector('.sectionTitle').textContent)
            $("#deleteSectionNum").text(section.querySelector('.numberOfSection').textContent)
            $("#deleteBtn").click(() => {
                if ($(".section").length > 1) {
                    removeSection($(section))
                    $("#deleteModal").modal('hide');
                }
            });
            $("#deleteModal").modal('show');
        });

        changed = true;
    }
}

function submitCourse() {
    if ($("#courseForm").find(".form-control").filter((i, el) => el.value == "").length > 0) {
        status("Info about course must be entered!", true)
        return;
    }

    $(".spinner-border").show();
    let sections = $(".section");
    let arrOfSections = [];

    for (var sect of sections) {
        let arrOfContents = [];
        let numSect = parseInt(sect.querySelector(".numberOfSection").textContent);

        arrOfSections.push(
            {
                Number: numSect,
                Title: $(sect).find($(`.sectionTitle`)).text(),
            }
        )
        let contents = sect.querySelectorAll(".content");

        for (var i of contents) {
            let num = parseInt(i.querySelector(".numberOfContent").textContent.replace(".", ""));
            let videoId = players[parseInt($(i).find(".ytVideo").attr("data-number"))].getVideoData()['video_id'];

            arrOfContents.push(
                {
                    Number: num,
                    Title: $(i).find($(`.contentTitle`)).text(),
                    Text: $(i).find($(".textContents")).val()
                }
            )

            if (videoId && !videoId.includes("emptylink")) {
                arrOfContents[arrOfContents.length - 1].YtVideoId = videoId;
                arrOfContents[arrOfContents.length - 1].Duration = Math.floor(players[parseInt($(i).find(".ytVideo").attr("data-number"))].getDuration());
            }

            var idContent = $(i).find($(".idContent")).val().trim();
            if (idContent) arrOfContents[arrOfContents.length - 1].Id = idContent;
        }

        var idSection = $(sect).find($(".idSection")).val().trim();
        if (idSection) {
            arrOfSections[arrOfSections.length - 1].SectionId = idSection;
        }

        arrOfSections[arrOfSections.length - 1].Contents = arrOfContents;
    }

    
    $.ajax({
        type: "POST",
        url: path + `/SectionsFunctionalities/${$("#idOfCourse").val()}`,
        traditional: true,
        data: JSON.stringify(arrOfSections),
        contentType: "application/json",
        success: (data) => {
            var keywords = $('.keyword').map(function () {
                return $.trim($(this).text().replace("x", ""));
            }).get().join(";");
            $("#keywords").val(keywords)
            
            $.ajax({
                type: "POST",
                url: path + `/UpdateCourse/${$("#idOfCourse").val()}`,
                traditional: true,
                data: new FormData($("#courseForm")[0]),
                contentType: false,
                processData: false, 
                success: function (response) {
                    status(response);
                },
                error: function (xhr) {
                    status(response, true);
                }
            });
        },
    }).done(() => { $(".spinner-border").hide(); });
    changed = false;
}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#courseImage')
                .attr('src', e.target.result);
        };


        reader.readAsDataURL(input.files[0]);
        $("#courseImageLabel").text(input.files[0].name);
    }
}

function removeSection(sectionEl = $()) {
    let sections = Array.from($(".section"));
    let section = sectionEl.find(".section");
    if (sections.length > 1) {
        sections = sections.slice(sections.findIndex(el => el.textContent == section.text()) + 1);

        let num = parseInt(section.find($(".numberOfSection")).text().trim());
        if (isNaN(num)) num = 1;
        sectionEl.remove();
        for (var sect of sections) {
            sect.querySelector(".numberOfSection").textContent = num;
            let text = sect.querySelector(".sectionTitle").textContent;
            sect.querySelector(".sectionTitle").textContent = text.includes("Section title") ? "Section title " + num : text;
            sect.querySelector(".sectionCollapseLink").setAttribute("href", `#section${num}`)
            let collapse = sect.querySelector(".sectionContents");
            collapse.setAttribute("id", `section${num++}`)

        }
    }
    else {
        section.parent().remove();
        addSection();
    }
    changed = true;
}

function removeContent(cont = $()) {
    let section = $(cont.parent().parent())
    let sectionNumber = section.find(".numberOfContent").text()
    let length = section.find(".content").length;
    
    if (length > 1) {
        let num = 1;
        cont.remove();
        let contents = section.find(".content");
        for (var content of contents) {
            content.querySelector(".numberOfContent").textContent = num;
            content.querySelector(".contentCollapseLink").setAttribute("href", `#content${sectionNumber}${num}`)
            content.querySelector(".contentCollapse").setAttribute("id", `content${sectionNumber}${num++}`)
        }
        section.find(".contentCollapse").collapse('hide');
    }
    else {
        section.find(".content").parent().remove();
        addContent(section)
    }
    changed = true;
}

$("#btnSave").click(() => {
    if (changed)
        submitCourse();
    else {
        status("Nothing to save", true);
    }
});

$("#publishForm").submit((e) => {
    var condition = $(".form-control").filter(function () {
        return $.trim($(this).val()).length == 0
    }).length == 0;

    if (!condition && $(".section").length < 5 || Array.from($(".section")).some(el => el.querySelectorAll(".content").length < 5) || $(".sectionTitle:empty").length != 0 || $(".contentTitle:empty").length != 0) {
        e.preventDefault();
        status("You can't publish course beacuse you must have at least 5 Sections and each of them must includes 5 Contents. Also, all inputs can't be empty.", true);
        return false;
    }
    else {
        submitCourse();
    }
});

$("#plusSection").click(addSection)

$(".addContent").click((e) => { addContent($(e.target).parent().parent()) });

$("#courseCategory").change((e) => {
    $.ajax({
        type: "GET",
        url: path + "/Subcategories/" + e.target.value,
        success: function (data) {
            $("#courseSubcategory").html("");

            for (var o of data) {
                $("#courseSubcategory").append(`<option value="${o.id}">${o.title}</option>`)
            }
        }
    });
});

document.onkeydown = SaveChanges;

$("[data-toggle='collapse']").on("show.bs.collapse", (e) => {
    console.log(e.target);
    $(e.target).find(".dropdown-toggle").toggleClass("rotate");
})

$(".btnTestVideo").click((e) => {
    testVideo($(e.target).parent().parent().parent().parent())
});

$(".deleteSection").click((e) => {
    let section = e.target.parentElement;
    $("#modalText").html(`Are you sure you want to delete section ${section.querySelector('.numberOfSection').textContent}. - "${section.querySelector('.sectionTitle').textContent}"?`)
    $("#deleteBtn").click(() => {
        removeSection($(section))
        $("#deleteModal").modal('hide');
    });
    $("#deleteModal").modal('show');
});

$(".deleteContent").click((e) => {
    let content = e.target.parentElement;
    $("#modalText").html(`Are you sure you want to delete content ${content.querySelector('.numberOfContent').textContent}. - "${content.querySelector('.contentTitle').textContent}"?`)
    $("#deleteBtn").click(() => {
        removeContent($(content))
        $("#deleteModal").modal('hide');
    });
    $("#deleteModal").modal('show');
});

selectElementContents($("#idOfCourse")[0]);
$(".spinner-border").hide();
