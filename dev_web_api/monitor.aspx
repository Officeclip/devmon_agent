<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="monitor.aspx.cs" Inherits="dev_web_api.monitor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>Name:</td>
                    <td><asp:TextBox ID="txtName" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Type:</td>
                    <td>
                        <asp:TextBox ID="txtType" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Arg1:</td>
                    <td><asp:TextBox ID="txtArg1" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Arg2:</td>
                    <td><asp:TextBox ID="txtArg2" runat="server"></asp:TextBox></td>
                </tr>
            </table>

            <asp:Button ID="Button1" runat="server" Text="save" OnClick="Button1_Click" />
            <br /><br />
        </div>
        <asp:HiddenField ID="HiddenField1" runat="server" />
        <asp:GridView ID="GridView1" runat="server"
            DataKeyNames="MonitorCommandId"
            OnRowEditing="GridView1_RowEditing"
            OnRowCancelingEdit="GridView1_RowCancelingEdit"
            OnRowDeleting="GridView1_RowDeleting"
            OnRowUpdating="GridView1_RowUpdating"
            CellPadding="4"
            ForeColor="#333333"
            GridLines="None">
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
        <div style="margin: 20px 0">
            <pre>
                Following are possible values of Types:
                -----------------------------------------
                url (arg1 -add the url in argument) - returns time to access the site
                cpu.percent - percentage cpu load 
                memory.free - amount of free memory in the system
                network.specific (arg1 - specific network name) (arg2 - ReceivedBytesPerSecond or SentBytesPerSecond) - Network load
                drive.free (arg1 - C:\, D:\ etc.) - Free space in the drive
                os.processes - number of processes
                os.uptime - amount of time the system is running without reboot
                os.pendingupdates - how many updates are pending
                os.lastupdated - time when the updates was last run
            </pre>
        </div>
    </form>
</body>
</html>
