<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="hardware.aspx.cs" Inherits="dev_web_api.hardware" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin: 20px">
            <div>
                <div style="display:inline">
                    Agents:
                    <asp:DropDownList ID="ddlAgents" runat="server"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlAgents_SelectedIndexChanged" />
                </div>
                <div style="display:inline; margin-left: 20px">
                    <asp:Literal ID="litDate" runat="server" />
                </div>
                <div style="display:inline; margin-left: 20px">
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
                <asp:TreeView ID="treeView1" runat="server">
                </asp:TreeView>
            </div>
        </div>
    </form>
</body>
</html>
