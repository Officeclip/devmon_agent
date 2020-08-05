<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="monitor.aspx.cs" Inherits="dev_web_api.monitor" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/spacings.min.css">
    <div class="heading">
        Monitors
    </div>
    <div>
        <div style="float:left" class="inputForm">
            <table>
                <tr>
                    <td style="color:red">* Name:</td>
                    <td>
                        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Type:</td>
                    <td>
                        <div>
                        <asp:DropDownList ID="ddlType" runat="server"
                            OnSelectedIndexChanged="ddlType_SelectedIndexChanged"
                            AutoPostBack="true">
                        </asp:DropDownList>
                            </div>
                        <div class="info">
                        <asp:Label ID="lblType" runat="server"></asp:Label>
                            </div>

                    </td>
                </tr>
                <tr>
                    <td>Arg1:</td>
                    <td>
                        <asp:TextBox ID="txtArg1" runat="server"></asp:TextBox>
                        <div class="info">
                        <asp:Label ID="lblArg1" runat="server"></asp:Label>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>Arg2:</td>

                    <td>
                        <asp:TextBox ID="txtArg2" runat="server"></asp:TextBox>
                        <div class="info">
                        <asp:Label ID="lblArg2" runat="server"></asp:Label>
                            </div>

                    </td>
                </tr>
                 <tr>
                    <td>Units:</td>

                    <td>                      
                        <asp:Label ID="lblUnit" runat="server"></asp:Label>

                    </td>
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
        <div style="float: left; width: 700px">
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <asp:GridView ID="GridView1" runat="server"
                AutoGenerateColumns="false"
                DataKeyNames="MonitorCommandId"
                OnRowEditing="GridView1_RowEditing"
                OnRowCancelingEdit="GridView1_RowCancelingEdit"
                OnRowDeleting="GridView1_RowDeleting"
                OnRowUpdating="GridView1_RowUpdating"
                OnRowDataBound="GridView1_RowDataBound"
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
                            <asp:Label ID="lblName" runat="server"
                                Text='<%#Eval("Name")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server"
                                Width="100"
                                Text='<%#Eval("Name")%>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <asp:Label ID="lblType" runat="server"
                                Text='<%#Eval("Type")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlTypes" runat="server"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlTypes_SelectedIndexChanged">

                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Arg1">
                        <ItemTemplate>
                            <asp:Label ID="lblArg1" runat="server"
                                Text='<%#Eval("Arg1")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtArg1" runat="server"
                                Width="150"
                                Text='<%#Eval("Arg1")%>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Arg2">
                        <ItemTemplate>
                            <asp:Label ID="lblArg2" runat="server"
                                Text='<%#Eval("Arg2")%>' />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtArg2" runat="server"
                                Width="150"
                                Text='<%#Eval("Arg2")%>' />
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Unit">
                        <ItemTemplate>
                            <asp:Label ID="lblUnit" runat="server"
                                Text='<%#Eval("Unit")%>' />
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
        <div style="clear: both" />
    </div>

    <div style="margin: 20px 0; display:none">
        <span class="heading">Monitor Command Help</span>
        <asp:GridView ID="grdMonitorHelp" runat="server" />
    </div>

</asp:Content>
