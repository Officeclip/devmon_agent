<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="commandLimits.aspx.cs" Inherits="dev_web_api.commandLimits" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="float: left">
                <h3>Monitor Command Limits</h3>
            </div>
            <div style="float: left; margin: 20px 0 0 20px">
                <asp:Button ID="btnBack" runat="server"
                    Text="Back to Monitor"
                    Font-Bold="true"
                    OnClick="btnBack_Click" />
            </div>
            <div style="clear: both" />
        </div>
        <div>
            <table>
                <tr>
                    <th>Type</th>
                    <th>Warning Limit</th>
                    <th>Error Limit</th>
                    <th>Is Low Limit?</th>
                </tr>
                <asp:Repeater ID="rptCommandLimits" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Label ID="lblType" runat="server"
                                    Text='<%# Eval("Type") %>' /></td>
                            <td>
                                <asp:TextBox ID="txtWarningLimit" runat="server"
                                    Text='<%# Eval("WarningLimit") %>' /></td>
                            <td>
                                <asp:TextBox ID="txtErrorLimit" runat="server"
                                    Text='<%# Eval("ErrorLimit") %>' /></td>
                            <td>
                                <asp:CheckBox ID="chkIsLowLimit" runat="server"
                                    Checked='<%# Eval("IsLowLimit") %>'
                                    Enabled="false" /></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
        <div>
            <asp:Button ID="btnSave" runat="server"
                Text="Submit"
                OnClick="btnSave_Click" />
        </div>
    </form>
</body>
</html>
