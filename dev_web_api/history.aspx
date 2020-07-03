<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="history.aspx.cs" Inherits="dev_web_api.history" %>

<html>

<head runat="server">
    <title></title>
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.13.0/moment.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.6.0/Chart.min.js"></script>
<%--    <script src="https://cdn.jsdelivr.net/npm/chart.js@2.8.0"></script>--%>

</head>
<body>
   <form id="form1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server">
 </asp:ScriptManager>
       <div style="margin: 20px">
            <div>
                <div style="display: inline">
                    Monitor Commands:
                <asp:DropDownList ID="ddlMonitorCommands" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlMonitorCommands_SelectedIndexChanged" />
                </div>
                <select id="ddlOne">
                    <option value="1">OfficeClip Ping</option>
                    <option value="2">Google Ping</option>
                </select>
                <div style="display:inline; margin-left: 20px">
                    <asp:Literal ID="litDate" runat="server" />
                </div>
                <div style="display: inline; margin-left: 20px">
                    <asp:Button ID="btnBack" runat="server"
                        Text="Back to Monitor"
                        Font-Bold="true" />
                </div>
            </div>
        <div style="margin-top: 10px">
            <asp:Label ID="lblEmptyData" runat="server"
                Text="Data for this agent is not available"
                ForeColor="Red" />
        </div>
        </div>
       <button id="btnGenerateChart" onclick="GenChart()">Create Chart</button>
    </form>
    <div style="width:800px; height: 800px">
    <canvas id="myChart"></canvas>
        </div>
    <script type="text/javascript">
        $(document).ready(function () {
            console.log("ready!");
            GenChart();
        });
        function GenChart() {
            $.ajax({
                type: "POST",
                url: "/monitor/history.aspx/GetChart",
                data: "{monitorCommandId: \"" + $("[id*=ddlOne]").val() + "\"}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess_,
                failure: OnErrorCall_
            });
        }

        function OnSuccess_(response) {
            //debugger;
            //alert("success: " + response.d);
            var jsonarray = JSON.parse(response.d);

            var labels = jsonarray.map(function (e) {
                return e.text;
            });
            var data = jsonarray.map(function (e) {
                return e.value;
            });;

            var ctx = $("#myChart").get(0).getContext("2d");
            var color = Chart.helpers.color;
            window.chartColors = {
                red: 'rgb(255, 99, 132)',
                orange: 'rgb(255, 159, 64)',
                yellow: 'rgb(255, 205, 86)',
                green: 'rgb(75, 192, 192)',
                blue: 'rgb(54, 162, 235)',
                purple: 'rgb(153, 102, 255)',
                grey: 'rgb(201, 203, 207)'
            };
            // see: https://www.chartjs.org/samples/latest/scales/time/line.html for syntax
            var config = {
                type: 'line',
                responsive: true,
                maintainAspectRatio: false,
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'SKD-Surface',
                        data: data,
                        backgroundColor: color(window.chartColors.red).alpha(0.5).rgbString(),
                        borderColor: window.chartColors.red,
                        fill: false
                    }]
                },
                options: {
                    scales: {
                        yAxes: [
                            {
                                ticks: {
                                    callback: function (value, index, values) {
                                        return value + ' ms';
                                    }
                                }
                            }
                        ],
                        xAxes: [
                            {
                                ticks: {
                                    display: true,
                                    beginAtZero: true,
                                    max: 60,
                                    maxTicksLimit: 12,
                                    callback: function (value, index, values) {
                                        if (value > 0) { value = -1 * value;}
                                        return value + ' min';
                                    }
                                }
                            }
                        ]
                    }
                }
            };

            var chart = new Chart(ctx, config);
        }

        function OnErrorCall_(response) {
            alert("Error: " + response)
        }

    </script>
</body>
</html>
