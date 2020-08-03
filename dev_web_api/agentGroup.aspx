<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="agentGroup.aspx.cs" Inherits="dev_web_api.agentGroup"
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function openPopUp(str) {
            prop = 'toolbar=0,location=0,scrollbars=1,height=400';
            prop += ',width=350,left=400,top=150,resizable=yes';
            window.open(str, 'popup window', prop).focus();
            //window.open('about.aspx');
        }
    </script>
    <style type="text/css">
        .pl {
            padding-left: 10px
        }

        .pt {
            padding-top: 10px
        }
    </style>

    <div>
        <div class="pt">
            <div>
                <div>
                </div>
                <div>
                    <table>
                        <tr>
                            <td class="pt">Group name:</td>
                            <td class="pl pt">
                                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>                               
                            </td>
                            <td class="pl pt">
                                 <asp:Label ID="lblmsg" runat="server" ForeColor="Red" ></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table class="pt">
                        <tr>
                            <td class=" pt pl">
                                <div class="pl" style="float: left">
                                    <asp:Button ID="btnSave" runat="server"
                                        Text="save"
                                        Font-Bold="true"
                                        OnClick="btnSave_Click" />
                                </div>
                            </td>
                            <td class=" pt pl">
                                <asp:Button ID="btnBack" runat="server"
                                    Text="Back to Monitor"
                                    Font-Bold="true"  OnClick="btnBack_Click"
                                     />
                            </td>
                        </tr>
                    </table>

                </div>
            </div>

        </div>
        <div class="pt">

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
