<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="dev_web_api.Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div>
                <span><b>1. Select the checkbox to generate the  real data with the agents and commands<br />
                    2. Other wise it will generates random data</b></span>
            </div>
            <div style="padding-top: 10px">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </div>
            <table>
                <tr>
                    <td>Minutes
                    </td>
                    <td>
                        <asp:TextBox ID="txtMin" runat="server" Width="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Hours
                    </td>
                    <td>
                        <asp:TextBox ID="txtHrs" runat="server" Width="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Days
                    </td>
                    <td>
                        <asp:TextBox ID="txtDays" runat="server" Width="50"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <asp:CheckBox ID="ckhSimualator" runat="server" Text="Generate orginal data" AutoPostBack="false" />
        </div>
        <div style="padding-top: 10px">

            <asp:Button ID="txtData" runat="server" Text="Generate Test Data" OnClick="txtData_Click" />
        </div>
        <hr />
        <div>
            Fix the history table with the current time so that the charts can be tested!
        </div>
        <div>
            <asp:Button id="fixHistoryData" runat="server" 
                Text="Fix History Data" OnClick="fixHistoryData_Click" />
        </div>
    </form>
</body>
</html>
