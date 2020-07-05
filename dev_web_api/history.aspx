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
                
                <div style="display: inline; margin-left: 20px">
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
        <div style="width: 800px; height: 800px">
            <canvas id="myChart"></canvas>
        </div>
         <script type="text/javascript">
             var ctx = $("#myChart").get(0).getContext("2d");
             var config = <%= chartConfigString %>;
             var chart = new Chart(ctx, config);
        </script>  
    </form>
 
</body>
</html>
