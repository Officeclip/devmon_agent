<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="monitor.aspx.cs" Inherits="dev_web_api.monitor" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .pl {
            padding-left: 10px
        }

        .pt {
            padding-top: 10px
        }
    </style>
    <div class="pt">
        <div>
            <div>
                <div style="float: right; padding-top: 20px; margin-left: 50px;">
                    <asp:Button ID="btnBack" runat="server"
                        Text="Back to Monitor"
                        Font-Bold="true"
                        OnClick="btnBack_Click" />
                </div>
            </div>
            <div>
                <table>
                    <tr>
                        <td>Name:</td>
                        <td class="pl">Type:</td>
                        <td class="pl">Arg1:</td>
                        <td class="pl">Arg2:</td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                        </td>
                        <td class="pl">
                            <asp:TextBox ID="txtType" runat="server"></asp:TextBox>

                        </td>
                        <td class="pl">
                            <asp:TextBox ID="txtArg1" runat="server"></asp:TextBox></td>
                        <td class="pl">
                            <asp:TextBox ID="txtArg2" runat="server"></asp:TextBox></td>
                        <td class="pl">
                            <asp:Button ID="Button1" runat="server" Text="save" OnClick="Button1_Click" />

                        </td>
                        <td class="pl">
                            <asp:Label ID="lblError" runat="server"
                                ForeColor="Red" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </div>

    <div class="pt">

        <asp:HiddenField ID="HiddenField1" runat="server" />
        <asp:GridView ID="GridView1" runat="server"
            DataKeyNames="MonitorCommandId"
            OnRowEditing="GridView1_RowEditing"
            OnRowCancelingEdit="GridView1_RowCancelingEdit"
            OnRowDeleting="GridView1_RowDeleting"
            OnRowUpdating="GridView1_RowUpdating"
            CellPadding="4"
            ForeColor="#333333"
            GridLines="None" Width="100%">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:CommandField ShowEditButton="True" />
                <asp:CommandField ShowDeleteButton="True" />
            </Columns>
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            <SortedAscendingCellStyle BackColor="#FDF5AC" />
            <SortedAscendingHeaderStyle BackColor="#4D0000" />
            <SortedDescendingCellStyle BackColor="#FCF6C0" />
            <SortedDescendingHeaderStyle BackColor="#820000" />
        </asp:GridView>
    </div>

    <div style="margin: 20px 0">
        <h3>Monitor Command Help</h3>
        <asp:GridView ID="grdMonitorHelp" runat="server" />
    </div>

</asp:Content>
