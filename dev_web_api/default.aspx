<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="dev_web_api._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="refresh" content="30">
    <title></title>
    <style>
        table, th, td {
            text-align: center;
            border: 1px solid #CCC;
        }

        th, td {
            padding: 5px;
        }

            td:nth-child(1), th {
                font-weight: bold;
                background-color: #EEE;
            }

        .warningLimit {
            background-color: lightgoldenrodyellow;
        }

        .errorLimit {
            background-color: lightcoral;
        }

        li{
            line-height: 1.5em;
        }
    </style>
    <script src="https://code.jquery.com/jquery-3.5.0.js"></script>
    <script type="text/javascript">
        //setInterval(function () {
        //    $('#my_div').load('http://localhost/monitor/default.aspx');
        //}, 2000) /* time in milliseconds (ie 2 seconds)*/
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <div style="margin: 20px 0">
            <div>
                Last Updated: <%= DateTime.Now %>
            </div>
            <table id="tblMonitor" runat="server" />
            <div style="margin: 20px">
                <asp:Button ID="btnPopup" runat="server"
                    Text="Edit Monitor"
                    OnClick="btnPopup_Click" />
                <asp:Button ID="btnCommandLimit" runat="server"
                    Text="Command Limits" OnClick="btnCommandLimit_Click" />
                <asp:Button ID="btnAlias" runat="server"
                    Text="Edit Agent"
                    OnClick="btnAlias_Click" />
            </div>
            <div style="margin: 20px">
                <asp:Button ID="btnHardware" runat="server"
                    Text="Hardware" 
                    OnClick="btnHardware_Click" />
                <asp:Button ID="btnSoftware" runat="server"
                    Text="Software" 
                    OnClick="btnSoftware_Click" />
            </div>
            <div style="margin:20px">
                <asp:Button ID="btnSignOff" runat="server"
                    Text="Sign Off"
                     OnClick="btnSignOff_Click" />
            </div>
            <div style="margin-top: 30px">
                <h3>Agent Installation:</h3>
                <ul>
                    <li>
                        Download and Install:
                             
                                <ol>
                                    <li>Download 
                                        <a 
                                            href="https://github.com/Officeclip/devmon_agent/releases/download/Release-0.5.2/AgentSetup.zip"
                                            style="font-weight:bold">AgentSetup.zip</a></li>
                                    <li>Unzip AgentSetup.zip</li>
                                    <li>Run <b>Setup.exe</b></li>
                                    <li>Enter Server Url: <b><%= GetWebUri() %></b></li>
                                    <li>Enter Server Key: <b><%= serverGuid %></b></li>
                                </ol>

                    </li>


                    <li>Uninstall:
                        <ul>
                            <li>
                                Go to the <i>Control Panel > Programs and Features</i>
                                and uninstall the Agent Monitor
                            </li>
                        </ul>
                        
                    </li>
                </ul>
            </div>
        </div>
    </form>
</body>
</html>
