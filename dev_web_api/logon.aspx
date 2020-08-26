<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logon.aspx.cs" Inherits="dev_web_api.logon" %>

<!DOCTYPE html>

<html>
<head id="Head1" runat="server">
    <title>Monitor - Login</title>
    <style type="text/css">
        .login {
            margin-left: auto;
            margin-right: auto;
        }

        input[type="submit" i]:hover {
            background-color: cornflowerblue
        }

        .login_input_box {
            background: #fff;
            border-radius: 10px;
            font-family: Arial,Helvetica,sans-serif;
            font-size: 16px;
            height: 34px;
            padding-left: 2px;
            width: 343px;
        }

        .backGround {
            border: 1px solid;
            background: aliceblue;
            height: 180px;
            border-radius: 10px;
        }

        .error {
            border: Solid 1px #daa520;
            background-color: #eee8aa;
            margin: 5px;
            padding: 5px;
            display: flex;
            max-width: 600px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="margin-left: 470px;">
            <h3>Logon to Monitor</h3>
        </div>

        <table class="login backGround ">
            <tr>
                <td style="font-size: 16px; padding-left: 20px; padding-top: 15px">E-mail address:</td>
                <td style="padding-top: 15px">
                    <asp:TextBox ID="UserEmail" CssClass="login_input_box" runat="server" /></td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                        ControlToValidate="UserEmail"
                        Display="Dynamic" ForeColor="Red"
                        ErrorMessage="Cannot be empty."
                        runat="server" />
                </td>
            </tr>
            <tr>
                <td style="font-size: 16px; padding-left: 20px;">Password:</td>
                <td>
                    <asp:TextBox ID="UserPass" CssClass="login_input_box" TextMode="Password"
                        runat="server" />
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                        ControlToValidate="UserPass" ForeColor="Red"
                        ErrorMessage="Cannot be empty."
                        runat="server" />
                </td>
            </tr>
            <tr>
                <td style="font-size: 16px; padding-left: 20px;">Remember me?</td>
                <td>
                    <asp:CheckBox ID="Persist" runat="server" /></td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="test" Visible="false" runat="server" />

                </td>
                <td>
                    <div>
                        <asp:Button ID="Submit1" runat="server" CssClass="login"
                            OnClick="Submit1_Click" Font-Bold="true" Height="35" Width="80"
                            Text="Log On" />
                    </div>
                </td>
            </tr>
        </table>

        <table class="login">
            <tr>
                <td>

                    <p>
                        <asp:Label ID="Msg" CssClass="error" ForeColor="red" runat="server" />
                    </p>
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
