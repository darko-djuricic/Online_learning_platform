let path = `${window.location.protocol}//${window.location.host}/api/Values`;
path += location.pathname.toLowerCase().includes("admin") ? "/AdminChart" : "/TeacherChart";
var ctx = document.getElementById('myChart').getContext('2d');
let chartData = [];
let background = [
    `rgb(${Math.floor(Math.random() * 256)},${Math.floor(Math.random() * 256)},${Math.floor(Math.random() * 256)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`,
    `rgb(${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)},${Math.floor(Math.random() * 186)})`
];
var chart;

function chartOnChange() {
    $(".spinner").show();
    if (chart) chart.destroy();
    let data = chartData.filter(el => el.id == $("#courseSelect").val() && el.year == $("#yearSelect").val())[0];
    if (data) {
        let order = $('#orderSelect').val();
        chart = new Chart(ctx, {
            type: $("#chartTypeSelect").val(),
            data: {
                labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'Oktober', 'November', 'December'],
                datasets: [{
                    label: `${order?order.charAt(0).toUpperCase() + order.slice(1):"Popularity"} of your courses in ${$("#yearSelect").val()}`,
                    backgroundColor: background,
                    data: data[order ? order:"popularity"]
                }]
            },
            options: {
                scales: {
                    yAxes: [
                        {
                            ticks: {
                                beginAtZero: true
                            },
                            scaleLabel: {
                                display: true,
                                labelString: order=="rating"?"Rating":"Participants+rating"
                            }
                        }
                    ]
                }
            }
        });
    }
    else {
        chart = new Chart(ctx, {
            type: $("#chartTypeSelect").val(),
            data: {
                labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'Oktober', 'November', 'December'],
                datasets: [{
                    label: `Popularity of your courses in ${$("#yearSelect").val()}`,
                }]
            },
            options: {
                scales: {
                    yAxes: [
                        {
                            ticks: {
                                beginAtZero: true
                            }
                        }
                    ]
                }
            }
        });
    }
    $(".spinner").hide();
}

$.ajax({
    type: "GET",
    url: path,
    success: function (response) {
        chartData = response;
    }
}).done(chartOnChange);

$("#chartTypeSelect").change(chartOnChange);
$("#yearSelect").change(chartOnChange);
$("#courseSelect").change(chartOnChange);
$('#orderSelect').change(chartOnChange);

