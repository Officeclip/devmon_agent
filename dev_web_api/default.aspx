<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="dev_web_api._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="grdMonitor" runat="server">
            </asp:GridView>
            <div style="margin: 20px">
                <asp:Button ID="btnPopup" runat="server"
                    Text="Edit Monitor"
                    OnClick="btnPopup_Click" />
            </div>
        </div>
    </form>
</body>
</html>
