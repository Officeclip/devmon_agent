<%@ Page 
        Language="C#" 
        MasterPageFile="~/Site.Master" 
        Title="Home" 
        AutoEventWireup="true" 
        CodeBehind="default.aspx.cs" 
        Inherits="dev_web_api._default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        table{
            margin-top: 10px;
        }
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

        .systemError {
            background-color: lightpink;
        }

        .notAvailable {
            background-color: lightblue;
        }

        .headerTitle {
            background-color: lightgray;
        }

        li {
            line-height: 1.5em;
        }
    </style>
    <link rel="stylesheet" href="css/countdown.css" />

    <div style="margin: 20px 0">
        <div>
            <div id="app" style="float:left"></div>
            <div style="margin: 5px 0 0 20px; float:left">
                Last Updated: <%= DateTime.Now %>
            </div>
            <div style="clear:both" />
        </div>
        <div>
            <div style="float:right">
                <asp:Button ID="btnTestData" runat="server" Text="Test" ToolTip="Click to generate test Data" OnClick="btnTestData_Click" />
            </div>
        </div>
        <table id="tblMonitor" runat="server" />

        <div style="margin-top: 30px">
            <h3>Agent Installation:</h3>
            <ul>
                <li>Download and Install:
                             
                            <ol>
                                <li>Download 
                                    <a
                                        href="https://github.com/Officeclip/devmon_agent/releases/download/Agent-0.5/AgentSetup.zip"
                                        style="font-weight: bold">AgentSetup.zip</a></li>
                                <li>Unzip AgentSetup.zip</li>
                                <li>Run <b>Setup.exe</b></li>
                                <li>Enter Server Url: <b><%= GetWebUri() %></b></li>
                                <li>Enter Server Key: <b><%= serverGuid %></b></li>
                            </ol>

                </li>


                <li>Uninstall:
                    <ul>
                        <li>Go to the <i>Control Panel > Programs and Features</i>
                            and uninstall the Agent Monitor
                        </li>
                    </ul>

                </li>
            </ul>
        </div>
    </div>

    <script src="js/countdown.js"></script>

</asp:Content>