<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Graphcontrol.ascx.cs" Inherits="dev_web_api.Graphcontrol" %>
<div style="margin: 20px">
    <div>
        <div style="display: inline">
            Monitor Commands:
                <asp:DropDownList ID="ddlMonitorCommands" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlMonitorCommands_SelectedIndexChanged" />
        </div>
        <div style="display: inline">
            History for:
            <asp:DropDownList ID="ddlFrequancy" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFrequancy_SelectedIndexChanged">
                <asp:ListItem Text="Last hour" Value="0"></asp:ListItem>
                <asp:ListItem Text="Last 24hrs" Value="1"></asp:ListItem>
                <asp:ListItem Text="Last month" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div style="display: inline">
            Agent Groups:
            <asp:DropDownList ID="ddlAgentGroups" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlAgents_SelectedIndexChanged">
            </asp:DropDownList>
        </div>

        <div style="display: inline; margin-left: 20px">
            <asp:Literal ID="litDate" runat="server" />
        </div>
        <div style="display: inline; margin-left: 20px">
            <asp:Button ID="btnBack" runat="server" OnClick="btnBack_Click"
                Text="Back to Monitor"
                Font-Bold="true" />
        </div>
    </div>
    <div style="margin-top: 10px">
        <asp:Label ID="lblEmptyData" runat="server" Visible="false"
            Text="Data for this agent is not available"
            ForeColor="Red" />
    </div>
</div>
<div style="width: 800px; height: 800px">
    <canvas id="myChart"></canvas>
</div>
<script type="text/javascript">
    var ctxDay = $("#myChart").get(0).getContext("2d");
    var config = <%= chartConfigStringForDay %>;
    var chartforDay = new Chart(ctxDay, config);
</script>
