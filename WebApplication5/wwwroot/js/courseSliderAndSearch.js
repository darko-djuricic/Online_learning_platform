 $(document).ready(function() {
    $('.autoWidth').lightSlider({
        autoWidth: true,
        enableDrag: true,
        pager: false,
        loop: false,
        onSliderLoad: function() {
            $('.autoWidth').removeClass('cs-hidden');
        } 
    });  

     $(".searchCourses").keyup((e) => {
         let courses = document.querySelectorAll(".course");
         $('.course').removeClass("searched");
         $(".course").hide();
         courses.forEach((el, i) => {
             let target = e.target.value.trim().toLowerCase();
             let title = el.querySelector(".courseTitle").textContent.trim().toLowerCase();
             let desc = el.querySelector(".courseDesc").textContent.trim().toLowerCase();
             let author = el.querySelector(".courseAuthor").textContent.trim().toLowerCase();
             let condition = title.search(target) > -1 || desc.search(target) > -1 || author.search(target) > -1
             $(el).addClass(condition ? "searched" : "");
         });

         $("#status").text($(".searched").length > 0 ? "" : "No results.");
         $(".searched").show();
     })

     $("#forw").click(() => {
         let scroll = $("#slideCategories").scrollLeft();
         $("#slideCategories").animate({ scrollLeft: scroll + 200 }, 200);
     })

     $("#back").click(() => {
         let scroll = $("#slideCategories").scrollLeft();
         $("#slideCategories").animate({ scrollLeft: scroll - 200 }, 200);
     })

     $("#slideCategories").scroll((e) => {
         $("#forw").show();
         $("#back").show();
         if ((e.target.scrollLeft + e.target.offsetWidth) >= e.target.scrollWidth) {
             $("#forw").hide();
         }

         if (e.target.scrollLeft == 0) {
             $("#back").hide();
         }
     })

     $("#slideCategories").scrollLeft(0);
     $("#back").hide();

  });