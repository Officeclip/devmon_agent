<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="dev_web_api._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="refresh" content="30">
    <title></title>
    <style>
        table, th, td { 
                text-align:center; 
                border: 1px solid #CCC;
            } 
            th, td { 
                padding: 5px; 
            } 
            td:nth-child(1), th {  
              font-weight: bold;
              background-color: #EEE;
            }
            .warningLimit{
                background-color:lightgoldenrodyellow;
            }
            .errorLimit{
                background-color: lightcoral;
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
            <table id="tblMonitor" runat="server"  />
            <div style="margin: 20px">
                <asp:Button ID="btnPopup" runat="server"
                    Text="Edit Monitor"
                    OnClick="btnPopup_Click" />
            </div>
            <div style="margin: 20px">
                <asp:Button ID="btnHardware" runat="server"
                    Text="Hardware" OnClick="btnHardware_Click" />
                <asp:Button ID="btnSoftware" runat="server"
                    Text="Software" OnClick="btnSoftware_Click" />
            </div>
            <div style="margin: 20px">
                To run the agent, do the following:
                <ul>
                    <li>
                        Go to <a href="https://github.com/Officeclip/devmon_agent/releases">Github Release Page</a>
                        and download to a folder and unzip it.
                    </li>
                    <li>
                        Add <b><%= serverGuid %></b> to the server_guid tag of the appSettings.json file
                    </li>
                    <li>
                        In appSettings.json file add <b>http://localhost<%= Page.Request.RawUrl %>api</b> to server_url key 
                    </li>
                    <li>
                        Now copy the above folder to the machine where you want to install the
                        Agent
                    </li>
                    <li>
                        Run the file devmon_agent.exe from a command prompt
                    </li>
                    <li>
                        This screen should show the agents running...
                    </li>
                </ul>
            </div>
        </div>
    </form>
</body>
</html>
