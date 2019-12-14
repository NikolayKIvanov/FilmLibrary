var pageNumber = 1;
var totalCount = 0;
var moviesPerPage = 5;
var maxPages = 1;
var moviesOnPage = 8;

function getMovies(pageN) {
    if (maxPages >= pageNumber) {
        $.ajax({
            type: 'get',
            url: "/Movies/GetMoviesByCategory",
            contentType: 'application/json',
            dataType: 'json',
            data: { "pageNumber": pageN, "moviesOnPage": moviesPerPage, "category": document.title },
            success: function (data) {
                pageNumber++;
                maxPages = Math.ceil(data.totalCount / moviesPerPage);
                buildPage(data.movies);
            },
            error: function () {
                alert('error');
            }
        });
    }
}

if (pageNumber === 1) {
    $(document).ready(getMovies(pageNumber));
}

$(window).scroll(function () {
    if ($(document).height() - $(window).height() === $(window).scrollTop() && document.getElementById("searchByCategoryFieldId").value === "") {
        getMovies(pageNumber);
    }
});

function buildPage(movies) {
    $("#mainContainer").append($("<div/>",
        {
            class: "container",
            id: "movies"
        }));

    $.each(movies, function (index, value) {
        $("#movies").append($("<div/>",
            {
                class: "row m-5"
            }).append($("<div/>",
                {

                }).append($("<a/>",
                    {
                        "href": "/Movies/SingleMovie/?imdbId=" + value.imdbId
                    }).append($("<img/>",
                        {
                            class: "mb-2",
                            src: "/images/" + value.poster,
                            width: "220",
                            height: "320",
                            onerror: "this.onerror = null;this.src = '/images/no+poster+available.jpg'"
                        }
                    )
                    )
                ), $("<div/>",
                    {
                        class: "col"
                    }).append($("<div class='mb-3'/>").html("<h5>" + value.title + "</h5>"),
                        $("<div class='mb-3'/>").text("Director: " + value.director),
                        $("<div class='mb-3'/>").text("Production: " + value.production),
                        $("<div class='mb-3'/>").text("Actors: " + value.actors),
                        $("<div class='mb-3'/>").text("IMDB Rating: " + value.rating)
                    )
            ), $("<hr/>")
        );
    });
}