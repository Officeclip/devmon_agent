<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="alias.aspx.cs" Inherits="dev_web_api.alias" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
</head>
<body>
    <form id="form1" runat="server">
                        <asp:Button ID="btnBack" runat="server"
                    Text="Back to Monitor"
                    Font-Bold="true"
                    OnClick="btnBack_Click"/>
        <div style="margin: 20px">
            <table cellspacing="5" cellpadding="5" style="border: 1px solid gray">
                <tr>
                    <th>Guid</th>
                    <th>Machine Name</th>
                    <th>Alias</th>
                    <th>Active</th>
                    <th></th>
                </tr>
                <asp:Repeater ID="rptAlias" runat="server" 
                    OnItemCommand="rptAlias_ItemCommand" 
                    OnItemCreated="rptAlias_ItemCreated">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:HiddenField ID="hdnAgentId" runat="server"
                                    Value='<%# Eval("AgentId") %>' />
                                <asp:Label ID="lblGuid" runat="server" 
                                    Text='<%# Eval("Guid") %>' />
                            </td>
                            <td>
                                <asp:Label ID="lblMachineName" runat="server" 
                                    Text='<%# Eval("MachineName") %>' />
                            </td>
                            <td>
                                <asp:TextBox ID="txtAlias" runat="server"
                                    Text='<%# Eval("Alias") %>' />
                            </td>
                            <td>
                                <asp:CheckBox ID="chkEnabled" runat="server"
                                    Checked='<%# Eval("Enabled") %>' />
                            </td>
                            <td>
                                <asp:LinkButton ID="lnkDelete" runat="server"
                                    CommandName="Delete"
                                    CommandArgument='<%# Eval("AgentId") %>'
                                    Text="Delete"/>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <div style="margin: 20px">
                <asp:Button ID="btnSave" runat="server" 
                    Text="Save"
                    OnClick="btnSave_Click" />
                &nbsp;
                <asp:HyperLink ID="hypCancel" runat="server" 
                    Text="Cancel"
                    NavigateUrl="~/default.aspx" />
            </div>
        </div>
    </form>
</body>
</html>
