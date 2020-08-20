<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddAgentsToGroups.aspx.cs" Inherits="dev_web_api.AddAgensToGroup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">        
        function openPopUp(str) {
            prop = 'toolbar=0,location=0,scrollbars=1,height=300';
            prop += ',width=350,left=0,top=0,resizable=yes';
            window.open(str, 'popup window', prop).focus();
            //window.open('about.aspx');
        }
    </script>
    <style type="text/css">
        .pt {
            padding-top: 15px;
        }

        .pl {
            padding-left: 15px
        }
    </style>
    <link rel="stylesheet" href="css/style.min.css" />
    <link rel="stylesheet" href="css/spacings.min.css" />
</head>

<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <table class="p-t-12 p-l-12 body-content">
            <tr>
                <td>
                    <div>
                        Add Agents to Group
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="grdAgents" runat="server" OnRowDataBound="grdAgents_RowDataBound" AutoGenerateColumns="false">
                        <Columns>
                            <asp:TemplateField HeaderText="Agent Name">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkAgent" runat="server" />
                                    <asp:HiddenField ID="hdnAgentId" runat="server" />
                                    <asp:Label ID="lblAgentName" runat="server" Text='<%#Eval("ScreenName")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <table class="p-t-12 p-l-12">
            <tr>
                <td>
                    <asp:Button ID="btnSave" runat="server"
                        Text="Save"
                        Font-Bold="true"
                        OnClick="btnSave_Click" />
                </td>
                <td>
                    <asp:Button ID="btnCancel" runat="server"
                        Text="Cancel"
                        Font-Bold="true"
                        OnClick="btnCancel_Click" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
