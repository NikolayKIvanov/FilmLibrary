function search() {
    //var searchText = $("#searchFieldId").val();

    var searchText = document.getElementById("searchByCategoryFieldId").value;
    var type = $("#lunch").val().split(" ")[1];

    if (searchText === "") {
        $("#movies").empty();
        pageNumber = 1;
        getMovies(pageNumber);
    } else {
        $.ajax({
            type: 'get',
            url: "/Movies/SearchMoviesInDatabase",
            contentType: 'application/json',
            dataType: "json",
            data: { "searchText": searchText, "type": type, "category": document.title },
            success: function (result) {
                $("#movies").empty();
                buildPage(result.movies);
            },
            error: function () {
                alert('error');
            }
        });
    }
}
// Block UI when submitting forms
