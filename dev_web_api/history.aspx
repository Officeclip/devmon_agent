<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="history.aspx.cs" Inherits="dev_web_api.history" %>

<html>

<head runat="server">
    <title></title>
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.6.0/Chart.min.js"></script>
<%--    <script src="https://cdn.jsdelivr.net/npm/chart.js@2.8.0"></script>--%>

<script type="text/javascript">

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
        var config = {
            responsive: true;
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Graph Line',
                    data: data,
                    backgroundColor: 'rgba(0, 119, 204, 0.3)'
                }]
            }
        };

        var chart = new Chart(ctx, config);
        //var arr = [];
        //$.each(aData, function (i, val) {
        //    var obj = {};
        //    alert(val);
        //    obj.value = val.value;
        //    obj.label = val.text;
        //    arr.push(obj);
        //});
        //var ctx = $("#myChart").get(0).getContext("2d");
        //var myPieChart = new Chart(ctx).Pie(arr);
    }

    function OnErrorCall_(response) {
        alert("Error: " + response)
    }

    </script>
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
            var config = {
                type: 'line',
                maintainAspectRatio: false,
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Graph Line',
                        data: data,
                        backgroundColor: 'rgba(0, 119, 204, 0.3)'
                    }]
                }
            };

            var chart = new Chart(ctx, config);
            //var arr = [];
            //$.each(aData, function (i, val) {
            //    var obj = {};
            //    alert(val);
            //    obj.value = val.value;
            //    obj.label = val.text;
            //    arr.push(obj);
            //});
            //var ctx = $("#myChart").get(0).getContext("2d");
            //var myPieChart = new Chart(ctx).Pie(arr);
        }

        function OnErrorCall_(response) {
            alert("Error: " + response)
        }

    </script>
</body>
</html>
