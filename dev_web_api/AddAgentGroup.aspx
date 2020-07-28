<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddAgentGroup.aspx.cs" Inherits="dev_web_api.AddAgentGroup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>
                        <span>Group name:</span>
                        <asp:TextBox ID="txtgrpName" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>s
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
