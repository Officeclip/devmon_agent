<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProtoTest.aspx.cs" Inherits="dev_web_api.ProtoTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="float: left; margin-left: 48px; border: 1px solid #eee; padding: 8px">
            <table class="profile">
                <tr>
                    <td>Name:</td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Type:</td>
                    <td>
                        <asp:DropDownList ID="ddlType" runat="server"
                            OnSelectedIndexChanged="ddlType_SelectedIndexChanged"
                            AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:Label ID="lblType" runat="server"></asp:Label>

                    </td>
                </tr>
                <tr>
                    <td>Arg1:</td>
                    <td>
                        <asp:TextBox ID="txtArg1" runat="server"></asp:TextBox>
                        <asp:Label ID="lblArg1" runat="server"></asp:Label>

                    </td>
                </tr>
                <tr>
                    <td>Arg2:</td>

                    <td>
                        <asp:TextBox ID="txtArg2" runat="server"></asp:TextBox>
                        <asp:Label ID="lblArg2" runat="server"></asp:Label>

                    </td>
                </tr>
            </table>
            <div style="padding: 16px">
                <div>
                    <%-- <asp:Button ID="Button1" runat="server"
                        Font-Bold="true"
                        Text="Add New"
                        OnClick="Button1_Click" />
                    <asp:Button ID="btnBack" runat="server"
                        class="m-l-16"
                        Text="Back to Monitor"
                        Font-Bold="true"
                        OnClick="btnBack_Click" />--%>
                </div>
                <div>
                    <asp:Label ID="lblError" runat="server"
                        ForeColor="Black" />
                </div>
            </div>
            <div style="clear: both" />
        </div>
    </form>
</body>
</html>
