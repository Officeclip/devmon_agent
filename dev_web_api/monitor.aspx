<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="monitor.aspx.cs" Inherits="dev_web_api.monitor" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/spacings.min.css">
    <div class="heading">
        Monitors
    </div>
    <div>
        <div style="float:left; width: 700px">
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
                GridLines="None" 
                Width="100%">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:CommandField 
                            ButtonType="Image" 
                            CancelImageUrl="~/images/delete.png" 
                            EditImageUrl="~/images/edit.png"
                            ShowEditButton="True" 
                            UpdateImageUrl="~/Images/save.png" />
                    <asp:CommandField 
                            ButtonType="Image" 
                            DeleteImageUrl="~/Images/delete.png" 
                            ShowDeleteButton="True" />
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%#Eval("Name")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server" 
                                 Width="100"
                                Text='<%#Eval("Name")%>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <asp:Label ID="lblType" runat="server" Text='<%#Eval("Type")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtType" runat="server" 
                                 Width="100"
                                Text='<%#Eval("Type")%>' />
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Arg1">
                        <ItemTemplate>
                            <asp:Label ID="lblArg1" runat="server" Text='<%#Eval("Arg1")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtArg1" runat="server" 
                                 Width="150"
                                Text='<%#Eval("Arg1")%>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Arg2">
                        <ItemTemplate>
                            <asp:Label ID="lblArg2" runat="server" Text='<%#Eval("Arg2")%>' />
                        </ItemTemplate>
                         <EditItemTemplate>
                            <asp:TextBox ID="txtArg2" runat="server" 
                                 Width="150"
                                Text='<%#Eval("Arg2")%>' />
                        </EditItemTemplate>
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
        <div style="float:left; margin-left: 48px; border: 1px solid #eee; padding: 8px">
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
                        <asp:TextBox ID="txtType" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Arg1:</td>
                    <td>
                        <asp:TextBox ID="txtArg1" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Arg2:</td>

                    <td>
                        <asp:TextBox ID="txtArg2" runat="server"></asp:TextBox></td>
                </tr>
            </table>
            <div style="padding: 16px">
                <div>
                    <asp:Button ID="Button1" runat="server"
                        Font-Bold="true"
                        Text="Add New"
                        OnClick="Button1_Click" />
                    <asp:Button ID="btnBack" runat="server"
                        class="m-l-16"
                        Text="Back to Monitor"
                        Font-Bold="true"
                        OnClick="btnBack_Click" />
                </div>
                <div>
                    <asp:Label ID="lblError" runat="server"
                        ForeColor="Black" />
                </div>
            </div>

        </div>
        <div style="clear:both" />
    </div>

    <div style="margin: 20px 0">
        <span class="heading">Monitor Command Help</span>
        <asp:GridView ID="grdMonitorHelp" runat="server" />
    </div>

</asp:Content>
