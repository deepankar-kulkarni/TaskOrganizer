var stageData = [];
$(document).ready(function () {

    
    $.ajax({
        type: 'POST',
        url: 'DashboardStageCount',
        contentType: "application/json",
        dataType: "json",
        success: function (result) {
            stageData = result.stageCount;
            $("#totalTasks").html(result.totalCount);
            var ctx = $("#stageCanvas").get(0).getContext("2d");

            dataset = {
                datasets: [{
                    data: stageData,
                    backgroundColor: [
                        'rgb(0, 0, 204)',
                        'rgb(102, 0, 0)',
                        'rgb(255, 128, 0)',
                        'rgb(255, 0, 0)',
                        'rgb(102, 204, 0)'
                    ]
                }],

                // These labels appear in the legend and in the tooltips when hovering different arcs
                labels: [
                    'Pending',
                    'On Hold',
                    'In Progress',
                    'Not Started',
                    'Completed',
                ]

            };

            var myDoughnutChart = new Chart(ctx, {
                type: 'doughnut',
                data: dataset

            });
        }
    });

    
});

