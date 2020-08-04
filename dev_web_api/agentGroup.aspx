﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="agentGroup.aspx.cs" Inherits="dev_web_api.agentGroup"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/spacings.min.css">
    <script type="text/javascript">
        function openPopUp(str) {
            prop = 'toolbar=0,location=0,scrollbars=1,height=400';
            prop += ',width=350,left=400,top=150,resizable=yes';
            window.open(str, 'popup window', prop).focus();
            //window.open('about.aspx');
        }
    </script>
    <style type="text/css">
        
    </style>

    <div style="width:55px">
        <div class="p-t-12">
            <div>
                <div>
                </div>

            </div>

        </div>
        <div style="float:right; border:1px solid gray">

            <table class="profile" >
                <tr>
                    <th>
                        Add new Group
                    </th>
                </tr>               
                <tr>                  
                    <td class="p-t-12">Group name:</td>
                    <td class="p-t-12">
                        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                    </td>
                    <td class="p-t-12">
                        <asp:Label ID="lblmsg" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="profile m-l-72">
                <tr>
                    <td>
                        <asp:Button ID="btnSave" runat="server"
                            Text="save"
                            Font-Bold="true"
                            OnClick="btnSave_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnBack" runat="server"
                            Text="Back to Monitor"
                            Font-Bold="true" OnClick="btnBack_Click" />
                    </td>
                </tr>
            </table>

        </div>                    
        <div class="p-t-12">
            <span><h5>Groups</h5></span>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <asp:GridView ID="grdGroups" runat="server" AutoGenerateColumns="false"
                DataKeyNames="AgentGroupId"
                OnRowEditing="grdGroups_RowEditing"
                OnRowCancelingEdit="grdGroups_RowCancelingEdit"
                OnRowDeleting="grdGroups_RowDeleting"
                OnRowUpdating="grdGroups_RowUpdating"
                OnRowDataBound="grdGroups_DataBound"
                CellPadding="4"
                ForeColor="#333333"
                GridLines="None" Width="40%">
                <AlternatingRowStyle BackColor="White" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" />
                    <asp:CommandField ShowDeleteButton="True" />
                    <asp:TemplateField HeaderText="Group name">
                        <ItemTemplate>
                            <asp:Label ID="lblGrpName" runat="server" Text='<%#Eval("AgentGroupName")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkAgents" runat="server" Text="Agents"></asp:HyperLink>
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

    </div>
</asp:Content>
