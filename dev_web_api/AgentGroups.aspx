﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgentGroups.aspx.cs" Inherits="dev_web_api.AddAgenstoGroup" %>

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
</head>

<body>
    <form id="form1" runat="server">
        <div>
           
        </div>
        <table class="pt pl">
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
                                    <asp:Label ID="lblAgentName" runat="server" Text='<%#Eval("MachineName")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <table class="pt pl">
            <tr>
                <td>
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                </td>
                <td>
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
