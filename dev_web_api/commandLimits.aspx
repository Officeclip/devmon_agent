﻿<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="commandLimits.aspx.cs" Inherits="dev_web_api.commandLimits" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/spacings.min.css">
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
        <div style="clear: both"></div>
    </div>
    <div class="inputForm" style="border: 1px solid gray; padding: 10px; width: 55%">
        <table style="margin-left: 15px;">
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
                           <%-- <asp:CheckBox ID="chkIsLowLimit" runat="server"
                                Checked='<%# Eval("IsLowLimit") %>'
                                Enabled="false" />--%>
                            <asp:Label ID="lblIsLimit" runat="server" Text='<%# GetText(Eval("IsLowLimit")) %>'></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <div style="margin-left: 15px; margin-bottom: 15px">
            <asp:Button ID="btnSave" runat="server"
                Text="Submit" Font-Bold="true"
                OnClick="btnSave_Click" />
        </div>
    </div>


</asp:Content>
