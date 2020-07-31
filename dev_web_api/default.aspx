﻿<%@ Page
    Language="C#"
    MasterPageFile="~/Site.Master"
    Title="Home"
    AutoEventWireup="true"
    CodeBehind="default.aspx.cs"
    Inherits="dev_web_api._default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/countdown.css" />

    <div>
        <div>
            <div id="app" style="float: left"></div>
            <div style="margin: 5px 0 0 20px; float: left">
                <span>Agent Group:</span>
                <asp:DropDownList ID="ddlAgentGroups"
                    runat="server"
                    OnSelectedIndexChanged="ddlAgentGroups_SelectedIndexChanged"
                    AutoPostBack="true">
                    <asp:ListItem Value="-1" Text="All groups" Selected="True">                            
                    </asp:ListItem>
                </asp:DropDownList>
                <asp:CheckBox ID="chkEmailOpt" runat="server" 
                    style="padding-left: 40px"
                    OnCheckedChanged="chkEmailOpt_CheckedChanged" 
                    AutoPostBack="true" />
                <span>Send Email when Agent fails</span> &nbsp&nbsp
                <span style="font-size:small;padding-left: 70px">
                    Last Updated: <%= DateTime.Now %>
                </span>
                
            </div>
            <div style="clear: both" />

        </div>

        <div>
            <div style="float: right">
                <asp:Button ID="btnTestData" runat="server" Text="Test" ToolTip="Click to generate test Data" OnClick="btnTestData_Click" />
            </div>
        </div>
        <table id="tblMonitor" class="monitor" runat="server" />

        <div style="margin-top: 50px">
            <span class="heading">Agent Installation:</span>
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
