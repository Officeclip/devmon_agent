<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="alias.aspx.cs" Inherits="dev_web_api.alias" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/spacings.min.css">
    <asp:Button ID="btnBack" runat="server"
        Text="Back to Monitor"
        Font-Bold="true"
        OnClick="btnBack_Click" />
    <div class="inputForm" style="margin: 20px; padding:10px; border: 1px solid gray; width:60%">
        <table  cellspacing="5" cellpadding="5" >
            <tr>

                <th>Guid</th>
                <th>Machine Name</th>
                <th>Alias</th>
                <th>Active</th>
                <th></th>
            </tr>
            <tr>
                <td>
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
                                        Text="Delete" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>


                </td>

            </tr>
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

</asp:Content>
