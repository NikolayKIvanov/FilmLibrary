// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.]
$("#searchBtnId").click(function () {
    if ($("form").valid()) {
        $("#mainContainer").html("<div class=\"d-flex justify-content-center\" id=\"loadingId\">" +
            "<div class=\"spinner-grow mx-1 text-danger\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n" +
            "</div>\r\n<div class=\"spinner-grow mx-1 text-warning\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n" +
            "</div>\r\n<div class=\"spinner-grow mx-1 text-success\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n</div>" +
            "</div>");
    }
});

$("#leftArrow").click(function () {
    $("#mainContainer").html("<div class=\"d-flex justify-content-center\" id=\"loadingId\">" +
            "<div class=\"spinner-grow mx-1 text-danger\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n" +
            "</div>\r\n<div class=\"spinner-grow mx-1 text-warning\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n" +
            "</div>\r\n<div class=\"spinner-grow mx-1 text-success\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n</div>" +
            "</div>");
});

$("#rightArrow").click(function () {
    $("#mainContainer").html("<div class=\"d-flex justify-content-center\" id=\"loadingId\">" +
            "<div class=\"spinner-grow mx-1 text-danger\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n" +
            "</div>\r\n<div class=\"spinner-grow mx-1 text-warning\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n" +
            "</div>\r\n<div class=\"spinner-grow mx-1 text-success\" role=\"status\">" +
            "\r\n<span class=\"sr-only\">Loading...</span>\r\n</div>" +
            "</div>");
});

function searchTitleInDatabase() {
    var searchText = document.getElementById("searchFieldId").value;

    if (searchText.length < 3) {
        $("#foundMovies").empty();
        $('#foundMovies').css('display', 'none');
    } else if(searchText.length >= 3){
        $.ajax({
            type: 'get',
            url: "/Movies/SearchMoviesInDatabase",
            contentType: 'application/json',
            dataType: "json",
            data: { "searchText": searchText, "type": "Title", "category": "All" },
            success: function (result) {
                $("#foundMovies").empty();
                buildSearchResult(result.movies);
            },
            error: function () {
                alert('error');
            }
        });
    }
}

function buildSearchResult(movies) {
    $('#foundMovies').empty();
    $('#foundMovies').css('display', 'block');

    $.each(movies, function (index, value) {
        $("#foundMovies").append($("<a/>",
                {
                    "href": "/Movies/SingleMovie/?imdbId=" + value.imdbId,
                    class: "container row my-1"
                }).append($("<div/>",
                    {

                    }).append($("<img/>",
                            {
                                src: "/images/" + value.poster,
                                width: "60",
                                onerror: "this.onerror = null;this.src = '/images/no+poster+available.jpg'"
                            }
                        )
                    ), $("<div/>",
                    {
                        class: "col"
                    }).append($("<div class='mb-3'/>").html("<strong>" + value.title + "</strong>"),
                    $("<div class='mb-3'/>").text("IMDB Rating: " + value.rating)
                )
            ), $("<hr/>")
        );
    });
}