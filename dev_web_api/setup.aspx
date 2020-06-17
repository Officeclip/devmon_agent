<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="setup.aspx.cs" Inherits="dev_web_api.setup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
</head>
<body>
    <form id="form1" runat="server">        
        <div style="margin: 20px">
            <h1>Setup Monitor</h1>
            <div>
                <div>
                    *Email: 
                    <asp:TextBox ID="txtEmail" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvUser" runat="server" 
                        ErrorMessage="Please enter Username" 
                        ControlToValidate="txtEmail" />
                </div>
                <div>
                    Password:
                    <asp:TextBox ID="txtPassword" runat="server"
                         TextMode="Password" />
                </div>
            </div>
            <div>
                <asp:Button ID="btnSend" runat="server"
                    Text="Save" 
                    OnClick="btnSend_Click"/>
            </div>
        </div>
    </form>
</body>
</html>
