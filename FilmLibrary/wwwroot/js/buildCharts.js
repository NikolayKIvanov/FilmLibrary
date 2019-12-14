var chartData;
var chartType;
var currentType;
var currentCategory;
var allNames = new Array();
var config;
var canvasP = document.getElementById('myChart');
var ctx = canvasP.getContext('2d');
var chart = new Chart(ctx, config);

function buildChart(data, type = 'pie') {
    $('#radarId').parent().removeClass('active');
    $('#typeId').text(currentType);

    $('#statisticsId').html('Most common <i>' +
        currentType +
        '</i> in <i>' +
        currentCategory +
        '</i><br><i>' +
        type.capitalize() +
        '</i> chart');

    chart.destroy();
    $('myChart').empty();
    $('.chartjs-size-monitor').empty();
    var genreCounts = new Array();
    allNames = new Array();
    var backgroundColor = [
        'rgba(255, 99, 132)',
        'rgba(54, 162, 235)',
        'rgba(255, 206, 86)',
        'rgba(75, 192, 192)',
        'rgba(168, 113, 235)'
    ];

    var options;
    if (type === 'bar') {
        options = {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        };
    }

    $.each(data,
        function (index, value) {
            allNames.push(value.name);
            genreCounts.push(value.count);
        });
    config = {
        type: type,
        data: {
            datasets: [{
                label: 'Most watched ' + currentType.toLowerCase() + ' - ' + data[0].name,
                data: genreCounts,
                backgroundColor: backgroundColor
            }],
            labels: allNames
        },
        options: options
    };

    chart = new Chart(ctx, config);
    window.chart = chart;
};

function getTop() {
    chartType = $("input[name='chartType']:checked").val();
    if (chartType === 'radar') {
        buildRadarChart();
    } else {
        $("input[name='chartType']:checked").parent().addClass('active');
        currentCategory = $("input[name='category']:checked").val();
        currentType = $("input[name='type']:checked").val();

        $.ajax({
            type: 'get',
            url: "/Statistics/GetTop" + currentType + "Count",
            contentType: 'application/json',
            dataType: 'json',
            data: { "category": currentCategory },
            success: function (data) {
                chartData = data;
                buildChart(chartData, chartType);
            },
            error: function () {
                alert('error');
            }
        });
    }
}

function buildRadarChart() {
    currentType = $("input[name='type']:checked").val();

    $('#chartGroupId').children().removeClass("active");
    $('#typeId').text(currentType);

    $('#statisticsId').html('Most common <i>' +
        currentType +
        '</i> in <i>All Categories</i><br><i>Radar</i> chart');

    chart.destroy();
    $('myChart').empty();
    $('.chartjs-size-monitor').empty();

    allNames = new Array();
    var favoritesDataset = new Array();
    var watchlistDataset = new Array();
    var watchedDataset = new Array();

    $.when(getFavoritesDataForType(), getWatchedDataForType(), getWatchlistDataForType()).done(function (favoritesData, watchedData, watchlistData) {
        $.each(favoritesData[0],
            function (index, value) {
                allNames.push(value.name);
            });

        $.each(watchedData[0],
            function (index, value) {
                allNames.push(value.name);
            });

        $.each(watchlistData[0],
            function (index, value) {
                allNames.push(value.name);
            });

        allNames = [...new Set(allNames)];

        $.each(allNames,
            function (index, value) {
                if (favoritesData[0].some(el => el.name === value)) {
                    favoritesDataset.push(favoritesData[0].find(x => x.name === value).count);
                } else {
                    favoritesDataset.push(0);
                }

                if (watchlistData[0].some(el => el.name === value)) {
                    watchlistDataset.push(watchlistData[0].find(x => x.name === value).count);
                } else {
                    watchlistDataset.push(0);
                }

                if (watchedData[0].some(el => el.name === value)) {
                    watchedDataset.push(watchedData[0].find(x => x.name === value).count);
                } else {
                    watchedDataset.push(0);
                }
            });

        config = {
            type: 'radar',
            data: {
                labels: allNames,
                datasets: [{
                    label: 'Most common ' + currentType + ' in Favorites',
                    backgroundColor: 'rgba(255, 99, 132, 0.3)',
                    data: favoritesDataset
                }, {
                    label: 'Most common ' + currentType + ' in Watchlist',
                    backgroundColor: 'rgba(54, 162, 235, 0.3)',
                    data: watchlistDataset
                }, {
                    label: 'Most common ' + currentType + ' in Watched',
                    backgroundColor: 'rgba(75, 192, 192, 0.3)',
                    data: watchedDataset
                }]
            },
            options: {
                scale: {
                    ticks: {
                        beginAtZero: true,
                    },
                    pointLabels: {
                        fontSize: 14
                    }
                }
            }
        };

        chart = new Chart(ctx, config);
        window.chart = chart;
    });
}

function getWatchedDataForType() {
    return $.ajax({
        type: 'get',
        url: "/Statistics/GetTop" + currentType + "Count",
        contentType: 'application/json',
        dataType: 'json',
        data: { "category": 'Watched' },
        error: function () {
            alert('error');
        }
    });
}

function getWatchlistDataForType() {
    return $.ajax({
        type: 'get',
        url: "/Statistics/GetTop" + currentType + "Count",
        contentType: 'application/json',
        dataType: 'json',
        data: { "category": 'Watchlist' },
        error: function () {
            alert('error');
        }
    });
}

function getFavoritesDataForType() {
    return $.ajax({
        type: 'get',
        url: "/Statistics/GetTop" + currentType + "Count",
        contentType: 'application/json',
        dataType: 'json',
        data: { "category": 'Favorites' },
        error: function () {
            alert('error');
        }
    });
}

window.onload = getTop();

String.prototype.capitalize = function () {
    return this.replace(/(?:^|\s)\S/g, function (a) { return a.toUpperCase(); });
};

canvasP.onclick = function (e) {
    var slice = chart.getElementAtEvent(e);
    if (!slice.length) return; // return if not clicked on slice
    var label = slice[0]._model.label;
    switch (label) {
        // add case for each label/slice
        case allNames[0]:
            alert('clicked on ' + label);
            break;
        case allNames[1]:
            alert('clicked on ' + label);
            break;
        case allNames[2]:
            alert('clicked on ' + label);
            break;
        case allNames[3]:
            alert('clicked on ' + label);
            break;
        case allNames[4]:
            alert('clicked on ' + label);
            break;
    }
}