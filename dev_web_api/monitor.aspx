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
            </div>
            <div style="background: #ce7e7e; width: 40%">
                <table>
                    <tr>
                        <td class="pt">Name:</td>
                        <td class="pl pt">
                            <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="pl pt">Type:</td>
                        <td class="pl pt">
                            <asp:TextBox ID="txtType" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="pl pt">Arg1:</td>
                        <td class="pl pt">
                            <asp:TextBox ID="txtArg1" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="pl pt">Arg2:</td>

                        <td class="pl pt">
                            <asp:TextBox ID="txtArg2" runat="server"></asp:TextBox></td>
                    </tr>
                </table>
                <table class="pt">
                    <tr>
                        <td class="pl pt">
                            <div class="pl" style="padding-left: 45px">
                                <asp:Button ID="Button1" runat="server"
                                    Font-Bold="true" 
                                    Text="save"
                                    OnClick="Button1_Click" />
                            </div>
                            
                        </td>
                        <td  class="pl pt">
                            <asp:Label ID="lblError" runat="server" 
                                ForeColor="Black" />
                        </td>
                        <td class="pl pt">
                            <asp:Button ID="btnBack" runat="server"
                                Text="Back to Monitor"
                                Font-Bold="true"
                                OnClick="btnBack_Click" />
                        </td>
                    </tr>
                </table>
                <div>
                    <div></div>
                </div>
            </div>
        </div>

    </div>

    <div class="pt">

        <asp:HiddenField ID="HiddenField1" runat="server" />
        <asp:GridView ID="GridView1" runat="server"
            AutoGenerateColumns="false"
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
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <asp:Label ID="lblName" runat="server" Text='<%#Eval("Name")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" Text='<%#Eval("Type")%>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Arg1">
                    <ItemTemplate>
                        <asp:Label ID="lblArg1" runat="server" Text='<%#Eval("Arg1")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Arg2">
                    <ItemTemplate>
                        <asp:Label ID="lblArg2" runat="server" Text='<%#Eval("Arg2")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Unit">
                    <ItemTemplate>
                        <asp:Label ID="lblUnit" runat="server" Text='<%#Eval("Unit")%>' />
                    </ItemTemplate>
                </asp:TemplateField>

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
