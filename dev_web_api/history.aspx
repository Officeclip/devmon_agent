<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="history.aspx.cs" Inherits="dev_web_api.history" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   <form id="form1" runat="server">
        <div style="margin: 20px">
            <div>
                <div style="display: inline">
                    Monitor Commands:
                <asp:DropDownList ID="ddlMonitorCommands" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlMonitorCommands_SelectedIndexChanged" />
                </div>
                <div style="display:inline; margin-left: 20px">
                    <asp:Literal ID="litDate" runat="server" />
                </div>
                <div style="display: inline; margin-left: 20px">
                    <asp:Button ID="btnBack" runat="server"
                        Text="Back to Monitor"
                        Font-Bold="true"
                        OnClick="btnBack_Click" />
                </div>
            </div>
        <div style="margin-top: 10px">
            <asp:Label ID="lblEmptyData" runat="server"
                Text="Data for this agent is not available"
                ForeColor="Red" />
            <div id="dvChart" />
            <div id="dvLegend" />
        </div>
        </div>
    </form>
</body>
</html>
